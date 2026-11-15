import type { User, Balance, HolidayRequest, ObstacleType } from "@/./Types";

const users: User[] = [
    { id: 1, email: 'user@ex.dk', username: 'user', role: 'employee' },
    { id: 2, email: 'accountant@ex.dk', username: 'accountant', role: 'accountant' },
    { id: 3, email: 'admin@ex.dk', username: 'admin', role: 'admin' },
];

const passwords: Record<number, string> = {
    1: "user123",
    2: "accountant123",
    3: "admin123",
};
export async function login (email: string, password: string) : Promise<User>
{
    const found = users.find((u) => u.email === email);
    if (found && passwords[found.id] === password) {
        return found;
    }
    else {
        throw new Error("Wrong email or password");
    }
}



const wait = () => new Promise((r) => setTimeout(r, 300));

const HOLIDAYS = ["2026-12-24", "2026-12-25", "2026-12-31"];

function hoursFor(date: Date): number {
    const d = date.getDay();
    if (d === 0 || d === 6) return 0;
    if (d === 5) return 7;
    return 7.5;
}

function isHoliday(s: string): boolean {
    return HOLIDAYS.includes(s);
}

export function costOfRange(from: string, to: string): { workdays: number; hours: number } {
    let workdays = 0;
    let hours = 0;
    const cur = new Date(from);
    const end = new Date(to);
    while (cur <= end) {
        const s = cur.toISOString().slice(0, 10);
        if (!isHoliday(s)) {
            const h = hoursFor(cur);
            if (h > 0) {
                workdays += 1;
                hours += h;
            }
        }
        cur.setDate(cur.getDate() + 1);
    }
    return { workdays, hours };
}

let balances: Balance[] = [
    { account: "weeks1_5", remainingHours: 96.5, remainingDays: 12.9, expiry: "31 Dec", flag: "5th week transferable by written agreement ≤ 31 Dec, weeks 1–4 → Feriefond" },
    { account: "week6", remainingHours: 22, remainingDays: 3, expiry: "30 Apr", flag: "Transferable ≤ 30 Sep, otherwise payout next salary" },
];

let requests: HolidayRequest[] = [
    { id: 1, account: "weeks1_5", from: "2026-07-06", to: "2026-07-10", workdays: 5, hours: 37, status: "approved", comment: "" },
    { id: 2, account: "week6", from: "2026-08-03", to: "2026-08-04", workdays: 2, hours: 15, status: "pending", comment: "" },
];
let nextId = 3;

export async function getMyBalances(): Promise<Balance[]> {
    await wait();
    return [...balances];
}

export async function getMyRequests(): Promise<HolidayRequest[]> {
    await wait();
    return [...requests];
}

export function getHolidays(): string[] {
    return [...HOLIDAYS];
}

export async function createRequest(account: HolidayRequest["account"], from: string, to: string): Promise<HolidayRequest> {
    await wait();
    const { workdays, hours } = costOfRange(from, to);
    const r: HolidayRequest = { id: nextId++, account, from, to, workdays, hours, status: "pending", comment: "" };
    requests.push(r);
    return r;
}

export async function reportObstacle(requestId: number, _type: ObstacleType, _from: string, _to: string, _docRef: string): Promise<void> {
    await wait();
    const r = requests.find((x) => x.id === requestId);
    if (!r) throw new Error("Request not found");
    r.comment = "Obstacle reported";
}
