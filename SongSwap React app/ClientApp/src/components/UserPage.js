import React, { Component } from 'react';
import Layout from './Layout';

export default class UserPage extends Component {
    static displayName = UserPage.name;

    constructor(props) {
        super(props);
        this.state = { uuid: '', integrationType: '' } // Corrected property name
    }

    componentDidMount() {
        const searchParams = new URLSearchParams(window.location.search);
        const data64 = searchParams.get('data64');

        if (data64) {
            const decodedData = atob(data64);

            try {
                const parsedData = JSON.parse(decodedData);

                if (parsedData.authModel.status === 'success') {
                    // Pass state along with navigation
                    
                    
                } else {
                    alert('Something went wrong');
                }
            } catch (error) {
                console.error('Error parsing JSON:', error);
            }
        }
    }



    render() {
        return (
            <Layout>
                <div>
                    <h1>This is user page!</h1>
                    <h2>User: {this.props.uuid}</h2>
                    <h2>Integration type: {this.props.integrationType}</h2> 
                </div>
            </Layout>
        );
    }
}
