import { createContext, useContext, useState, useCallback, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';

const UserContext = createContext();

export const UserProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const navigate = useNavigate();

    const loginUser = useCallback( async () => {
        try {
            console.log("Fetching user data");
            const response = await fetch('https://localhost:7089/api/user', {
                method: "GET",
                credentials: 'include',
            });

            if (response.status === 401) {
                console.log("Unathorized");
                alert("Reauthorization required")
                navigate('/');
            }

            const data = await response.json();
            setUser(data);
        } catch (error) {
            console.log(error);
        }
    }, [navigate]);

    const logoutUser = useCallback( async () => {
        console.log("logging out");
        fetch('https://localhost:7089/api/user/logout', {
            method: "POST",
            credentials: 'include'
        });
        setUser(null);
        navigate('/');
    }, [navigate]);

    const contextValue = useMemo(() => ({ user, loginUser, logoutUser }), [user, loginUser, logoutUser]);

    return (
        <UserContext.Provider value={ contextValue }>
            {children}
        </UserContext.Provider>
    );
};

export const useUser = () => useContext(UserContext);