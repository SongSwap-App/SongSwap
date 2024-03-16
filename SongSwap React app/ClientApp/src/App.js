import React from 'react';
import { Routes, Route } from 'react-router-dom';
import HomePage from './components/HomePage';
import PlaylistPage from './components/Playlist';
import Callback from './components/Callback';
import { Layout } from './components/Layout';
import { UserProvider } from './components/UserContext';


export default function App() {
    return (
        <UserProvider>
            <Layout>
                <Routes>
                    <Route path="/" element={<HomePage />} />
                    <Route path="/playlist" element={<PlaylistPage />} />
                    <Route path="/callback" element={<Callback />} />
                </Routes>
            </Layout>
        </UserProvider>
    );
}

