import React from 'react';
import { Routes, Route } from 'react-router-dom';
import HomePage from './components/HomePage';
import UserPage from './components/UserPage';
import Callback from './components/Callback';

export default function App() {
    return (
        <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/:id" element={<UserPage />} />
            <Route path="/callback" element={ <Callback /> } />
        </Routes>
    );
}

