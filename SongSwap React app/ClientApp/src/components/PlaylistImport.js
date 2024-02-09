import "./Playlist.css"
import React, { useState } from 'react';
import Spinner from 'react-bootstrap/Spinner';
import propTypes from 'prop-types';
import { useUser } from './UserContext'


const PlaylistImport = ({ playlist }) => {
    const [loading, setLoading] = useState(true);
    const [tracks, setTracks] = useState(null);
    const { user } = useUser();

    const importPlaylist = async () => {
        try {
            console.log("Importing playlist");
            const response = await fetch(`https://localhost:7089/api/playlist/import/${encodeURIComponent(playlist.id)}`, {
                method: "POST",
                credentials: 'include',
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            alert(`Playlist ${playlist.name} was successfully imported to ${user.destination}`);
        } catch (error) {
            console.error('Error fetching data:', error);
        }
    }

    const toggleShowHideTracks = async () => {
        const trackListElement = document.getElementById(playlist.id);
        if (trackListElement.style.display === "none") {
            trackListElement.style.display = "block";
        } else {
            trackListElement.style.display = "none";
        }
        if (!tracks && loading) {
            try {
                console.log("Fetching track list");
                const response = await fetch(`https://localhost:7089/api/playlist/${encodeURIComponent(playlist.id)}`, {
                    method: "GET",
                    credentials: 'include'
                });

                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }

                const data = await response.json();
                setTracks(data);
                setLoading(false);
            } catch (error) {
                console.error('error fetching track list:', error);
            }
        }
    }

    const renderTracks = () => {
        if (loading) {
            return (<div className="spinner-div">
                <Spinner animation="border" role="output">
                    <span className="visually-hidden">Loading...</span>
                </Spinner>
            </div>);
        } else if (!loading && !tracks) {
            return <span>Something went wrong...</span>
        }

        const trackListItems = tracks.map(track =>
            <div className="row" key={track.id}>
                <span>{ track.name }</span>
            </div>
        );

        return <div className="container">{trackListItems}</div>;
    }


    return (
        <div >
            <div className="playlist">
                <button onClick={ toggleShowHideTracks }>Tracks</button>
                <div className="col-md">
                    <h5>{playlist.name}</h5>
                </div>
                <div className="col-" id="totalItems">
                    <h5>Total Items: {playlist.totalItems}</h5>
                </div>
                <div className="col-">
                    <button className="btn btn-primary import" onClick={ importPlaylist }>Import</button>
                </div>
            </div>
            <div id={playlist.id} style={{display: "none"}} >
                { renderTracks() }
            </div>
        </div>
    );
};

PlaylistImport.propTypes = {
    playlist: propTypes.object,
};

export default PlaylistImport;