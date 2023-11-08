import React from 'react';
import "./Layout.css"

const Layout = ({ children }) => {
    return (
        <div>
            <header>
                <img src="SongSwap_Logo.jpg" alt="logo" className="logo" />
            </header>
            <main>
                {children}
            </main>
            <footer>
                <a href="https://github.com/Oordii/SongSwap" target="_blank">GitHub</a>
            </footer>
        </div>
    );
};

export default Layout;
