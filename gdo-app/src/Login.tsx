import {
    default as React,
    useState,
    useCallback,
    ChangeEvent,
    MouseEvent
} from 'react';
import { ryobiClient } from './RyobiClient';
import { withRouter, RouteComponentProps } from 'react-router';

export function LoginSectionBase(props: RouteComponentProps<{}>) {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const handleEmailChange = useCallback((e: ChangeEvent<HTMLInputElement>) =>
        setEmail(e.currentTarget.value)
    );

    const handlePasswordChange = useCallback(
        (e: ChangeEvent<HTMLInputElement>) => setPassword(e.currentTarget.value)
    );

    const handleSubmit = useCallback(
        async (e: MouseEvent<HTMLButtonElement>) => {
            setError('');
            const success = await ryobiClient.login(email, password);
            if (!success) {
                setError('The email address or password is invalid');
            } else {
                props.history.push('/');
            }
        }
    );

    return (
        <div className="container">
            {error && <div className="alert alert-danger">{error}</div>}
            <div className="form-group">
                <label>Email</label>
                <input
                    type="email"
                    className="form-control"
                    onChange={handleEmailChange}
                    value={email}
                />
            </div>
            <div className="form-group">
                <label>Password</label>
                <input
                    type="password"
                    className="form-control"
                    onChange={handlePasswordChange}
                />
            </div>
            <div className="form-group">
                <button
                    type="submit"
                    className="btn btn-primary btn-block"
                    onClick={handleSubmit}
                >
                    Login
                </button>
            </div>
        </div>
    );
}

export const LoginSection = withRouter(LoginSectionBase);
