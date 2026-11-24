import { useState } from "react";
import type { HolidayRequest, ObstacleType } from "@/Types.ts";
import { reportObstacle } from "@/api/FakeApi.ts";

type Props = { request: HolidayRequest; onClose: () => void; onSaved: () => void };

export function ObstacleModal({ request, onClose, onSaved }: Props) {
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