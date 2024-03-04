import React, { useState, useEffect } from 'react';
import Spinner from 'react-bootstrap/Spinner';
import PlaylistImport from './PlaylistImport';
import { useUser } from './UserContext';
import './Playlist.css';

const PlaylistPage = () => {
    const [playlists, setPlaylists] = useState(null);
    const [loading, setLoading] = useState(true);
    const { user, token, loginUser } = useUser();
    

    useEffect(() => {
        loginUser();
        const populatePlaylistData = async () => {
            try {
                console.log("Fetching playlist data");
                console.log(`Fetching ${process.env.REACT_APP_SERVER_HOST_URL}`)
                const response = await fetch(`${process.env.REACT_APP_SERVER_HOST_URL}/api/playlist`, {
                    method: "GET",
                    credentials: 'include',
                    headers: {
                        'Access-Control-Allow-Origin': '*',
                        'Access-Control-Allow-Headers': 'Origin, X-Requested-With, Content-Type, Accept',
                        'Authorization': `Bearer ${token}`
                    },
                });

                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }

                const data = await response.json();
                setPlaylists(data);
                setLoading(false);
            } catch (error) {
                console.error('Error fetching data:', error);
                setLoading(false);
            }
        };

        populatePlaylistData();
    }, []);

    const renderPlaylist = (playlists) => {
        const playlistsItems = playlists.map(playlist =>
            <div className="row" key={playlist.id}>
                <PlaylistImport playlist={ playlist } />
            </div>
        );

        return <div className="container">{playlistsItems}</div>;
    };

    const contents = () => {

        if (loading) {
            return (<div className="spinner-div">
                        <Spinner animation="border" role="output">
                            <span className="visually-hidden">Loading...</span>
                        </Spinner>
                    </div>);
        } else if (!loading && !playlists) {
            return (<h1>Something went wrong, go back to homepage</h1>);
        }

        const content = renderPlaylist(playlists);

        return (content);
    }

    const selectedPlatforms = () => {
        if (!user) {
            return (<div className="selected-platforms">
                <div className="spinner-div">
                    <Spinner animation="border" role="output">
                        <span className="visually-hidden">Loading...</span>
                    </Spinner>
                </div>)
            </div>);
        } else {
            return (<div className="selected-platforms">
                <img alt={user.source} src={`${user.source}_logo.png`} className={`platform-logo-${user.source}`} />
                <div className="arrow">
                    <img alt="arrow" src="arrow.png" className="arrow" />
                </div>
                <img alt={user.destination} src={`${user.destination}_logo.png`} className={`platform-logo-${user.destination}`} />
            </div>);
        }
    }

    
    return (
        <div className="container">
            <h1>Your playlists</h1>
            { selectedPlatforms() }
            { contents()  }
        </div>
    );
};

export default PlaylistPage;
