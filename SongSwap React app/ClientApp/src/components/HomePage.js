import React, { Component } from 'react';
import Layout from './Layout'; 

export default class HomePage extends Component {
    static displayName = HomePage.name;
    constructor(props) {
        super(props);
        this.state = { uuid: '', integrationType: '' }
    }

    Callback() {
        const searchParams = new URLSearchParams(window.location.search);
        const data64 = searchParams.get('data64');

        if (data64) {
            const decodedData = atob(data64);

            try {
                const parsedData = JSON.parse(decodedData);

                if (parsedData.authModel.status === 'success') {
                    // Pass state along with navigation
                    this.setState({ uuid: parsedData.authModel.uuid });
                    this.setState({ integrationType: parsedData.integration.type });
                    return true;

                } else {
                    alert('Something went wrong');
                    return false;
                }
            } catch (error) {
                console.error('Error parsing JSON:', error);
            }
        }
    }

    Prerender() {
        if (this.Callback()) {
            return(
                <div>
                    <h1>This is user page!</h1>
                    <h2>User: {this.state.uuid}</h2>
                    <h2>Integration type: {this.state.integrationType}</h2>
                </div>);
        }
        else {
            return (
                <div>
                    <a href="https://app.musicapi.com/songswap?returnUrl=https://localhost:44418/">
                        <button>Select playlist source</button>
                    </a>
                </div>
            )
        }
    }

    render() { 
        const content = this.Prerender();
        return (
        <Layout>
                { content }
        </Layout>
    );
  }
}
