import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

function Login() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (event) => {
        event.preventDefault();
        try {
            const response = await axios.post('https://localhost:7229/api/login', { UserName: username, Password: password });

            const token = response.data.accessToken;

            localStorage.setItem('token', token);

            navigate('/mainpage');
        } catch (error) {
            console.error('Login failed:', error.response.data);
        }
    };

    return (
        <div style={mainContainerStyle}>
            <div style={loginBoxStyle}>
                <h2 style={titleStyle}>Login</h2>
                <form onSubmit={handleSubmit} style={formStyle}>
                    <label htmlFor="username" style={labelStyle}>Username</label>
                    <input
                        id="username"
                        type="text"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        placeholder="Enter your username"
                        style={inputStyle}
                        required
                    />
                    <label htmlFor="password" style={{ ...labelStyle, marginTop: '10px' }}>Password</label>
                    <input
                        id="password"
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="Enter your password"
                        style={inputStyle}
                        required
                    />
                    <button type="submit" style={buttonStyle}>Login</button>
                </form>
            </div>
        </div>
    );
}

const mainContainerStyle = {
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    minHeight: '100vh',
    backgroundColor: '#121212',
    width: '100%',
    padding: 0,
};

const formStyle = {
    display: 'flex',
    flexDirection: 'column',
    width: '300px'
};

const labelStyle = {
    marginBottom: '5px'
};

const inputStyle = {
    padding: '10px',
    marginBottom: '10px',
    border: '1px solid #ccc',
    borderRadius: '4px',
    width: '70%',
    boxSizing: 'border-box'
};

const buttonStyle = {
    padding: '10px',
    border: 'none',
    borderRadius: '4px',
    backgroundColor: '#5CDB95',
    color: 'white',
    cursor: 'pointer',
    fontSize: '16px',
    marginTop: '10px',
    width: '70%',
};

const loginBoxStyle = {
    backgroundColor: '#A9A9A9',
    padding: '40px',
    borderRadius: '8px',
    boxShadow: '0 4px 8px rgba(0, 0, 0, 0.1)',
    width: '300px',
    boxSizing: 'border-box',
};

const titleStyle = {
    marginBottom: '20px',
    textAlign: 'center',
};

export default Login;
