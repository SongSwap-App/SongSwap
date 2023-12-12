import React from 'react';
import "./Layout.css"

const Layout = ({ children }) => {
    return (
        <div>
            <header>
                <a href="https://localhost:44418/">
                    <img src="SongSwap_Logo.jpg" alt="logo" className="logo" />
                </a>
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

export default Layout;
