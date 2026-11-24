import { useState } from "react";
import { ObstacleModal } from "@/components/ObstacleModal.tsx";
import type { HolidayRequest } from "@/Types.ts";

type Props = { requests: HolidayRequest[]; onSaved: (r: HolidayRequest) => void };

export function RequestsTable({ requests, onSaved }: Props) {
    const [reportFor, setReportFor] = useState<HolidayRequest | null>(null);
    
    return (
        <>
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
                                    <button className="btn btn-outline btn-sm" onClick={() => setReportFor(r)}>
                                        Report illness / obstacle
                                    </button>
                                )}
                            </td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            </div>
            {reportFor && <ObstacleModal request={reportFor} onClose={() => setReportFor(null)} onSaved={() => setReportFor(null)} />}
        </>
    );
}