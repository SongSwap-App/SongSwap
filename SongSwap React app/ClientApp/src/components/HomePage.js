import React from 'react';
import { useUser } from './UserContext';
import { Link } from 'react-router-dom';
import './Homepage.css';
import PlatformSelector from './PlatformSelector';

const HomePage = () => {
    const { user } = useUser();

    let contents;

    if (user) {
        contents =
            <div className="home">
                <Link to='/playlist'>
                    <button type="button" className="btn btn-primary">
                        Go back to started session
                    </button>
                </Link>
                <PlatformSelector newImport={true}/>
            </div>
    } else {
        contents =
            <div className="home">
                <h1>Import your playlists to any music platform</h1>
                <PlatformSelector newImport={false}/>
            </div>
    }

    return (contents);
};

export default HomePage;
