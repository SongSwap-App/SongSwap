import React, { useState } from 'react';
import { useUser } from './UserContext';
import './Homepage.css';  

export const PlatformButton = ({ platform, selected, onClick, logo }) => (
    <button
        type="button"
        className={`btn ${selected ? 'active' : ''}`}
        onClick={onClick}
    >
        <img alt={platform} src={logo} className={`platform-logo-${platform}`} />
    </button>
);

const PlatformSelector = ({ newImport }) => {
    const [selectedFrom, setSelectedFrom] = useState(null);
    const [selectedTo, setSelectedTo] = useState(null);
    const { user, logoutUser } = useUser();

    const handlePlatformClick = (platform, column) => {
        if (column === 'from') {
            setSelectedFrom(platform);

            if (selectedTo === platform) {
                setSelectedTo(null);
            }

        } else if (column === 'to') {
            setSelectedTo(platform === selectedTo ? null : platform);

            if (selectedFrom === platform) {
                setSelectedFrom(null);
            }            
        }
    };

    const startImport = () => {
        if (newImport) {
            logoutUser();
        }

        if (selectedFrom && selectedTo && selectedFrom !== selectedTo) {
            window.location.href = `https://app.musicapi.com/songswap/${selectedFrom}/auth?returnUrl=https://localhost:7089/api/user/callback/source?dest=${selectedTo}`;
        } else {
            alert('Please select valid source and destination platforms.');
        }
    };

    const importButton = () => {
        let text;
        if (newImport) {
            text = "Start new import"
        } else {
            text = "Start import"
        }


        if (selectedFrom && selectedTo) {
            return <button type="button" className="btn" onClick={startImport}>
                { text }
            </button>;
        } else {
            return <button type="button" className="btn disabled" onClick={startImport}>
                { text }
            </button>;
        }
    }

    return (
        <div className="container">
            <div className="selector">
                <div className="container platforms">
                    <div>
                        <h2>From</h2>
                    </div>
                    <PlatformButton
                        platform="youtube"
                        selected={selectedFrom === 'youtube'}
                        onClick={() => handlePlatformClick('youtube', 'from')}
                        logo="youtube_logo.png"
                    />
                    <PlatformButton
                        platform="spotify"
                        selected={selectedFrom === 'spotify'}
                        onClick={() => handlePlatformClick('spotify', 'from')}
                        logo="spotify_logo.webp"
                    />
                    <PlatformButton
                        platform="apple"
                        selected={selectedFrom === 'apple'}
                        onClick={() => handlePlatformClick('apple', 'from')}
                        logo="apple_logo.png"
                    />
                    {/* Add other buttons for the 'From' column */}
                </div>
                <div className="arrow">
                    <img alt="arrow" src="arrow.png"/>
                </div>
                <div className="container platforms">
                    <div>
                        <h2>To</h2>
                    </div>
                    <PlatformButton
                        platform="youtube"
                        selected={selectedTo === 'youtube'}
                        onClick={() => handlePlatformClick('youtube', 'to')}
                        logo="youtube_logo.png"
                    />
                    <PlatformButton
                        platform="spotify"
                        selected={selectedTo === 'spotify'}
                        onClick={() => handlePlatformClick('spotify', 'to')}
                        logo="spotify_logo.webp"
                    />
                    <PlatformButton
                        platform="apple"
                        selected={selectedTo === 'apple'}
                        onClick={() => handlePlatformClick('apple', 'to')}
                        logo="apple_logo.png"
                    />
                    {/* Add other buttons for the 'To' column */}
                </div>
            </div>
            { importButton() }
        </div>
    );
};

export default PlatformSelector;
