import React, { Component } from 'react';
import Layout from './Layout'; 

export default class HomePage extends Component {
  static displayName = HomePage.name;

    render() { 
    return (
        <Layout>
          <div>
                <a href="https://app.musicapi.com/songswap?returnUrl=https://localhost:44418/callback">
                    <button>Select a source of playlists</button>
                </a>
          </div>
        </Layout>
    );
  }
}
