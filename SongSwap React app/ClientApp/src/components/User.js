import React, { useEffect } from 'react';
import Dropdown from 'react-bootstrap/Dropdown';
import DropdownButton from 'react-bootstrap/DropdownButton';
import { useUser } from './UserContext';
import './User.css'

export const User = () => {
    const { user, logoutUser, loginUser } = useUser();

    useEffect(() => {
        
        if (!user) {
            loginUser();
        }
    });

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