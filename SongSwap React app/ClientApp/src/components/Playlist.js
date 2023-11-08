import React, { Component } from 'react';

export class Playlist extends Component {
    static displayName = Playlist.name;

    constructor(props) {
        super(props);
        this.state = { playlist: null, loading: true };
    }

    componentDidMount() {
        this.populatePlaylistData();
    }

    static renderPlaylist(playlist) {
        return (
            <div>
                <h3>Name: {playlist.name}</h3>
                <p>ID: {playlist.id}</p>
                <p>Total Items: {playlist.totalItems}</p>
                <p>Is Owner: {playlist.isOwner ? 'Yes' : 'No'}</p>
            </div>
        );
    }

    render() {
        let contents;
        if (this.state.loading) {
            contents = <p><em>Loading...</em></p>;
        } else if (this.state.playlist) {
            contents = Playlist.renderPlaylist(this.state.playlist);
        } else {
            contents = <p>Data not available.</p>;
        }


        return (
            <div>
                <h1>Playlist Data</h1>
                <p>This component demonstrates fetching playlist data from the server.</p>
                {contents}
            </div>
        );
    }

    async populatePlaylistData() {
        try {
            console.log("fecthing data");
            const response = await fetch('https://localhost:7089/api/playlist');
            console.log(response);
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            const data = await response.json();
            console.log(data);
            this.setState({ playlist: data, loading: false });
        } catch (error) {
            console.error('Error fetching data:', error);
            this.setState({ loading: false });
        }
    }
}
