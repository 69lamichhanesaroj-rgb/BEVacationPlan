import { useState, type FormEvent } from "react";
import { login } from "@/api/FakeApi.ts";
import type {User} from "@/Types.ts";

type Props = { onLogin: (user: User) => void };

export function LoginPage({ onLogin }: Props) {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    async function handleSubmit(e: FormEvent) {
        e.preventDefault();
        setError(null);
        setLoading(true);

        try {
            const user = await login(email, password);
            onLogin(user);
        } catch (err) {
            setError("Invalid email or password");
        } finally {
            setLoading(false);
        }
    }
    
    function handleShowPassword() {
        const passwordInput = document.getElementById("password") as HTMLInputElement | null;
        if (passwordInput) {
            passwordInput.type = passwordInput.type === "password" ? "text" : "password";
        }
    }
    
    return (
        <main className="login">
            <h1>BUSINESS ESBJERG</h1>
            
            <p style={{visibility: error? "visible": "hidden"}} className="error">{error? error: "placeholder"}</p>
            
            <form onSubmit={handleSubmit}>
                <label>
                    <h3>Email: </h3>
                    <input className="login-input"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        placeholder="Enter email"
                    />
                </label>

                <label>
                    <h3>Password: </h3>
                    <input
                        className="login-input"
                        id="password"
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="Enter password"
                    />
                </label>
                <div className="login-checkbox">
                    <input
                        type="checkbox"
                        id="show-password"
                        onClick={handleShowPassword}
                        style={{ marginRight: "6px" }}
                    />Show password
                </div>
                <button className="btn btn-primary" type="submit" disabled={loading}>
                    {loading ? "Logging in..." : "Log in"}
                </button>
            </form>
        </main>
    );
}
