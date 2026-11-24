import type { Balance } from "@/Types";

export function BalanceCards({ balances }: { balances: Balance[] }) {
    return (
        <div className="cards">
            {balances.map((b) => (
                <section key={b.account} className="card">
                    <h2>{b.account === "weeks1_5" ? "Weeks 1–5" : "Week 6"}</h2>
                    <p>{b.remainingHours}h ({b.remainingDays}d) — expires {b.expiry}</p>
                    <p>{b.flag}</p>
                </section>
            ))}
        </div>
    );
}