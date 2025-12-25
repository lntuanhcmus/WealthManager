export interface LoginRequest {
    email: string;
    password: string;
}

// Add Refresh Token definition
export interface RefreshToken {
    id: string;
    token: string;
    expires: string;
}

export interface AuthResponse {
    id: string;
    userName: string;
    email: string;
    roles: string[];
    isVerified: boolean;
    jwToken: string;
    refreshToken: RefreshToken;
}
