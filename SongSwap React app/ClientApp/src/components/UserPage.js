import React, { Component } from 'react';
import Layout from './Layout';

export default class UserPage extends Component {
    static displayName = UserPage.name;

    constructor(props) {
        super(props);
        this.state = { playlists: null, loading: true, user: null };
    }

    render() { 
    return (
        <Layout>
            <div>
            <h1>This is user page!</h1>
            </div>
        </Layout>
    );
  }
}
