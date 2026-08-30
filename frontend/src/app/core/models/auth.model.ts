export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  email: string;
  role: string;
  expiresAt: string;
}
