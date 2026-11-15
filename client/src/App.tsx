import "./index.css";

import {LoginPage} from "@/pages/loginPage.tsx";
import {useState} from "react";
import type {User} from "@/Types.ts";
import {UserDashboard} from "@/pages/UserDashboard.tsx";

export function App() {
    const [user, setUser] = useState <User | null>(null);
    
    if (user == null) return <LoginPage onLogin={setUser} />;
    
    if (user.role == 'employee') return <UserDashboard me={user} onLogout={() => {setUser(null);}} />
    else return (
        <main>
            <div style={{textAlign: 'center'}}>
                <h1>Admin and accountant dashboard is not implemented yet</h1>
                <input type="button" value="Logout" onClick={() => {setUser(null);}} />
            </div>
        </main>
    )
}

export default App;
