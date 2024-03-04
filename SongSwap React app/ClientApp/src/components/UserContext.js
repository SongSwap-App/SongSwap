import { createContext, useContext, useState, useCallback, useMemo, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const UserContext = createContext();

export const UserProvider = ({ children }) => {
    const [token, setToken] = useState(localStorage.getItem('token') || '');
    const [user, setUser] = useState(JSON.parse(localStorage.getItem('user')) || null);
    const navigate = useNavigate();

    useEffect(() => {
        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify(user));
    }, [token, user]);

    const setWebToken = useCallback(async (webToken) => {
        setToken(webToken);
    }, []);

    const loginUser = useCallback(async () => {
        try {
            const response = await fetch(`/api/user`, {
                method: "GET",
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.status === 401) {
                console.log("Unauthorized");
                alert("Authorization required");
                navigate('/');
            }

            const data = await response.json();
            setUser(data);
        } catch (error) {
            console.log(error);
        }
    }, [navigate, token]);

    const logoutUser = useCallback(async () => {
        fetch(`/api/user/logout`, {
            method: "POST",
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        setUser(null);
        setToken('');
        navigate('/');
    }, [navigate, token]);

    const contextValue = useMemo(() => ({ user, loginUser, logoutUser, token, setWebToken }), [user, loginUser, logoutUser, token, setWebToken]);

    return (
        <UserContext.Provider value={contextValue}>
            {children}
        </UserContext.Provider>
    );
};

export const useUser = () => useContext(UserContext);
