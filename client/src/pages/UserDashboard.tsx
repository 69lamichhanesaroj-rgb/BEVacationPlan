import { useEffect, useState } from "react";
import type { Account, Balance, HolidayRequest, ObstacleType, User } from "../Types";
import { costOfRange, createRequest, getMyBalances, getMyRequests, reportObstacle } from "../api/FakeApi";
import { TopBar } from "../components/TopBar";
import { HolidayCalendar } from "../components/HolidayCalendar";

type Props = { me: User; onLogout: () => void };

export function UserDashboard({ me, onLogout }: Props) {
  const [tab, setTab] = useState("Dashboard");
  const [balances, setBalances] = useState<Balance[]>([]);
  const [requests, setRequests] = useState<HolidayRequest[]>([]);
  const [day, setDay] = useState<string | null>(null);
  const [reportFor, setReportFor] = useState<HolidayRequest | null>(null);

  async function load() {
    setBalances(await getMyBalances());
    setRequests(await getMyRequests());
  }

  useEffect(() => {
    load();
  }, []);

  return (
    <main>
      <TopBar username={me.username} tabs={["Dashboard", "Calendar & Requests"]} active={tab} onTab={setTab} onLogout={onLogout} />
      {tab === "Dashboard" ? (
        <BalanceCards balances={balances} />
      ) : (
        <>
          <HolidayCalendar requests={requests} onDayClick={setDay} />
          {day && <RequestModal day={day} balances={balances} onClose={() => setDay(null)} onSaved={load} />}
          <RequestsTable requests={requests} onReport={setReportFor} />
          {reportFor && <ObstacleModal request={reportFor} onClose={() => setReportFor(null)} onSaved={load} />}
        </>
      )}
    </main>
  );
}

function BalanceCards({ balances }: { balances: Balance[] }) {
  return (
    <div className="cards">
      {balances.map((b) => (
        <section key={b.account} className="card">
          <h2>{b.account === "weeks1_5" ? "Weeks 1–5" : "Week 6"}</h2>
          <p>
            {b.remainingHours}h ({b.remainingDays}d) — expires {b.expiry}
          </p>
          <p>{b.flag}</p>
        </section>
      ))}
    </div>
  );
}

function RequestsTable({ requests, onReport }: { requests: HolidayRequest[]; onReport: (r: HolidayRequest) => void }) {
  return (
    <div className="table-wrap">
      <table className="table">
        <thead>
          <tr>
            <th>Dates</th>
            <th>Account</th>
            <th>Days</th>
            <th>Hours</th>
            <th>Status</th>
            <th>Comment</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {requests.map((r) => (
            <tr key={r.id}>
              <td>
                {r.from}–{r.to}
              </td>
              <td>{r.account === "weeks1_5" ? "1–5" : "6"}</td>
              <td>{r.workdays}</td>
              <td>{r.hours}</td>
              <td className="capitalize">{r.status}</td>
              <td>{r.comment}</td>
              <td>
                {r.status === "approved" && (
                  <button className="btn btn-outline btn-sm" onClick={() => onReport(r)}>
                    Report illness / obstacle
                  </button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function RequestModal({ day, balances, onClose, onSaved }: { day: string; balances: Balance[]; onClose: () => void; onSaved: () => void }) {
  const [account, setAccount] = useState<Account>("weeks1_5");
  const [from, setFrom] = useState(day);
  const [to, setTo] = useState(day);
  const [error, setError] = useState<string | null>(null);

  const { workdays, hours } = costOfRange(from, to);
  const balance = balances.find((b) => b.account === account);

  async function submit() {
    setError(null);
    if (balance && hours > balance.remainingHours) {
      setError("Insufficient balance in this account");
      return;
    }
    await createRequest(account, from, to);
    onSaved();
    onClose();
  }

  return (
    <div className="modal">
      <div className="modal-box">
        <h3>New request</h3>
        <label>
          Account{" "}
          <select value={account} onChange={(e) => setAccount(e.target.value as Account)}>
            <option value="weeks1_5">Weeks 1–5</option>
            <option value="week6">Week 6</option>
          </select>
        </label>
        <label>
          From <input type="date" value={from} onChange={(e) => setFrom(e.target.value)} />
        </label>
        <label>
          To <input type="date" value={to} onChange={(e) => setTo(e.target.value)} />
        </label>
        <p>
          {workdays} workday{workdays === 1 ? "" : "s"} = {hours}h
        </p>
        {error && <p className="error">{error}</p>}
        <div className="modal-actions">
          <button className="btn btn-primary" onClick={submit}>Submit</button>
          <button className="btn btn-ghost" onClick={onClose}>Cancel</button>
        </div>
      </div>
    </div>
  );
}

function ObstacleModal({ request, onClose, onSaved }: { request: HolidayRequest; onClose: () => void; onSaved: () => void }) {
  const [type, setType] = useState<ObstacleType>("illness-during");
  const [from, setFrom] = useState(request.from);
  const [to, setTo] = useState(request.from);
  const [docRef, setDocRef] = useState("");

  async function submit() {
    await reportObstacle(request.id, type, from, to, docRef);
    onSaved();
    onClose();
  }

  return (
    <div className="modal">
      <div className="modal-box">
        <h3>Report illness / obstacle</h3>
        <select value={type} onChange={(e) => setType(e.target.value as ObstacleType)}>
          <option value="illness-before">Ill before start</option>
          <option value="illness-during">Ill during holiday</option>
          <option value="parental">Parental leave</option>
          <option value="strike">Strike</option>
          <option value="other">Other</option>
        </select>
        <label>
          From <input type="date" value={from} onChange={(e) => setFrom(e.target.value)} />
        </label>
        <label>
          To <input type="date" value={to} onChange={(e) => setTo(e.target.value)} />
        </label>
        <label>
          Documentation ref <input value={docRef} onChange={(e) => setDocRef(e.target.value)} />
        </label>
        <p>Weeks 1–5 replacement after 5 sick days. Week 6 only in special cases.</p>
        <div className="modal-actions">
          <button className="btn btn-primary" onClick={submit}>Save</button>
          <button className="btn btn-ghost" onClick={onClose}>Cancel</button>
        </div>
      </div>
    </div>
  );
}
