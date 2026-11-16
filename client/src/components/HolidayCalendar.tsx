import { useState } from "react";
import { DayPicker, type DayButtonProps } from "react-day-picker";
import type { HolidayRequest } from "../Types";
import { getHolidays } from "../api/FakeApi";

type Props = {
  requests: HolidayRequest[];
  onDayClick: (date: string) => void;
};

function pad(n: number): string {
  return String(n).padStart(2, "0");
}

function toIso(date: Date): string {
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
}

function startOfMonth(date: Date): Date {
  return new Date(date.getFullYear(), date.getMonth(), 1);
}

function CalendarDay({ day: _day, children, modifiers, ...props }: DayButtonProps) {
  const marker = modifiers.approved ? "approved" : modifiers.pending ? "pending" : null;

  return (
    <button {...props}>
      {children}
      {marker && <span className={`cal-dot ${marker}`} aria-hidden="true" />}
    </button>
  );
}

const THIS_YEAR = new Date().getFullYear();

export function HolidayCalendar({ requests, onDayClick }: Props) {
  const [month, setMonth] = useState(() => startOfMonth(new Date()));
  const holidays = getHolidays();

  function covers(date: Date, status: HolidayRequest["status"]): boolean {
    const iso = toIso(date);
    return requests.some((r) => r.status === status && iso >= r.from && iso <= r.to);
  }

  return (
    <section className="calendar-section">
      <div className="cal-head">
        <div>
          <h3>Holiday calendar</h3>
          <p className="hint">Click a day to request holiday</p>
        </div>
        <button className="btn btn-primary" onClick={() => setMonth(startOfMonth(new Date()))}>Today</button>
      </div>

      <DayPicker
        className="holiday-calendar"
        month={month}
        onMonthChange={setMonth}
        startMonth={new Date(THIS_YEAR - 1, 0)}
        endMonth={new Date(THIS_YEAR + 2, 11)}
        captionLayout="dropdown"
        navLayout="around"
        weekStartsOn={1}
        showOutsideDays={false}
        modifiers={{
          holiday: (date: Date) => holidays.includes(toIso(date)),
          approved: (date: Date) => covers(date, "approved"),
          pending: (date: Date) => covers(date, "pending"),
          weekend: { dayOfWeek: [0, 6] },
        }}
        modifiersClassNames={{
          holiday: "cal-holiday",
          approved: "cal-approved",
          pending: "cal-pending",
          weekend: "cal-weekend",
        }}
        components={{ DayButton: CalendarDay }}
        onDayClick={(date) => onDayClick(toIso(date))}
        footer={
          <div className="legend">
            <span>
              <i className="cal-swatch approved" /> Approved
            </span>
            <span>
              <i className="cal-swatch pending" /> Pending
            </span>
            <span>
              <i className="cal-swatch holiday" /> Holiday / company day
            </span>
          </div>
        }
      />
    </section>
  );
}


