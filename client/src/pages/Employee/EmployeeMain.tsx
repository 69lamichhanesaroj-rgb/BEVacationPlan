import { useEffect, useState } from "react";
import type { Balance, HolidayRequest, User } from "@/Types.ts";
import { getMyBalances, getMyRequests } from "@/api/FakeApi.ts";
import { TopBar } from "@/components/TopBar.tsx";
import { EmployeeDashboard } from "@/pages/Employee/EmployeeDashboard.tsx";
import { EmployeeCalendar } from "@/pages/Employee/EmployeeCalendar.tsx";

const Pages = ["Dashboard", "Calendar"] as const;
type Tab = (typeof Pages)[number];

type Props = { me: User; onLogout: () => void };

export function EmployeeMain({ me, onLogout }: Props) {
    const [tab, setTab] = useState<Tab>("Dashboard");
    const [balances, setBalances] = useState<Balance[]>([]);
    const [requests, setRequests] = useState<HolidayRequest[]>([]);
    
    async function load() {
        setBalances(await getMyBalances());
        setRequests(await getMyRequests());
    }
    
    useEffect(() => {
        load();
    }, []);
    
    return (
        <main>
            <TopBar username={me.username} tabs={[...Pages]} active={tab} onTab={(t) => setTab(t as Tab)} onLogout={onLogout} />
            {tab === "Dashboard" ? (
                <EmployeeDashboard balances={balances} requests={requests} onSaved={load} />
            ) : (
                <EmployeeCalendar requests={requests} balances={balances} onSaved={load} />
            )}
        </main>
    );
}