import React from 'react';
import { useUser } from './UserContext';
import './User.css'

export const User = () => {
    const { user, logoutUser } = useUser();

    return (

        user ? (
            <div className="user">
                <label>{user.name}</label>
                <button type="button" className="btn btn-light" onClick={logoutUser}>Logout</button>
            </div>
        ) : (
            null
        )
    );
};

export default User;