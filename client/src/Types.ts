export type Role = "employee" | "accountant" | "admin";
export interface User { id:number;email: string; username:string; role:Role }

export type Account = "weeks1_5" | "week6";

export interface Balance {
    account: Account;
    remainingHours: number;
    remainingDays: number;
    expiry: string;
    flag: string;
}

export interface HolidayRequest {
    id: number;
    account: Account;
    from: string;
    to: string;
    workdays: number;
    hours: number;
    status: "pending" | "approved" | "rejected";
    comment: string;
}

export type ObstacleType = "illness-before" | "illness-during" | "parental" | "strike" | "other";