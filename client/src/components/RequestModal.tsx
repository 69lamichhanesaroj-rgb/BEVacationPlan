import { useState } from "react";
import type { Account, Balance } from "@/Types";
import  { costOfRange, createRequest } from "@/api/FakeApi";

type Props = { day: string; balances: Balance[]; onClose: () => void; onSaved: () => void };

export function RequestModal({ day, balances, onClose, onSaved }: Props) {
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