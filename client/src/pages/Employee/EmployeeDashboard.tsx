import type {Balance, HolidayRequest} from "@/Types.ts";
import { BalanceCards } from "@/components/BalanceCards.tsx";
import {RequestsTable} from "@/components/RequestsTable.tsx";

type Props = { balances: Balance[]; requests: HolidayRequest[]; onSaved: () => void };

export function EmployeeDashboard({ balances, requests, onSaved }: Props) {
    return (
        <>
            <BalanceCards balances={balances} />
            <RequestsTable requests={requests} onSaved={onSaved} />
        </>
    );
}