import React from 'react';
import "./Layout.css"
import User from './User';
import { Link } from 'react-router-dom';

export const Layout = ({ children }) => {

    return (
        <div>
            <header>
                <div className="container-fluid">
                    <Link to="/" className="logo">
                        <img src="SongSwap_Logo.jpg" alt="logo" className="logo" />
                    </Link>
                    <User />
                </div>
            </header>
            <main>
                {children}
            </main>
            <footer>
                <a href="https://github.com/Oordii/SongSwap">GitHub</a>
            </footer>
        </div>
    );
};

