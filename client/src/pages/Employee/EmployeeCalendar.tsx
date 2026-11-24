import { useState } from "react";
import type { Balance, HolidayRequest } from "@/Types";
import { HolidayCalendar } from "@/components/HolidayCalendar";
import { RequestModal } from "@/components/RequestModal";
import { RequestsTable } from "@/components/RequestsTable";

type Props = { requests : HolidayRequest[]; balances: Balance[]; onSaved: () => void };

export function EmployeeCalendar({ requests, balances, onSaved }: Props) {
    const [selectedDay, setSelectedDay] = useState<string | null>(null);
    return (
        <>
            <HolidayCalendar requests={requests} onDayClick={setSelectedDay} />
            {selectedDay && <RequestModal day={selectedDay} balances={balances} onClose={() => setSelectedDay(null)} onSaved={onSaved} />}
            <RequestsTable requests={requests} onSaved={onSaved} />
        </>
    );
}