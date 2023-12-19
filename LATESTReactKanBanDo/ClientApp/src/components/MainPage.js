import React, { useState, useEffect } from 'react';
import axios from 'axios';

function MainPage() {
    const [views, setViews] = useState([]);
    const [selectedView, setSelectedView] = useState(null);
    const [columns, setColumns] = useState([]);
    const [tasks, setTasks] = useState({});

    // Axios instance with the JWT included in the Authorization header
    const authAxios = axios.create({
        headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
        }
    });

    // Fetch Views
    useEffect(() => {
        const fetchViews = async () => {
            try {
                const response = await authAxios.get('https://localhost:7229/api/views');
                setViews(response.data);
                if (response.data.length > 0) {
                    selectView(response.data[0].id);
                }
            } catch (error) {
                console.error('Failed to fetch views:', error);
            }
        };

        fetchViews();
    }, []); // Empty dependency array to run only once on mount

    const selectView = async (viewId) => {
        setSelectedView(viewId);
        try {
            const columnsResponse = await authAxios.get(`https://localhost:7229/api/views/${viewId}/columns`);
            setColumns(columnsResponse.data);

            const newTasksByColumn = {};

            for (const column of columnsResponse.data) {
                const tasksResponse = await authAxios.get(`https://localhost:7229/api/views/${viewId}/columns/${column.id}/tasks`);
                newTasksByColumn[column.id] = tasksResponse.data;
            }

            setTasks(newTasksByColumn);

        } catch (error) {
            console.error('Failed to fetch columns or tasks:', error);
        }
    };

    const pageStyle = {
        backgroundColor: '#f0f0f0', // Light gray theme
        minHeight: '100vh',
        color: '#333', // Dark text for contrast
        padding: '20px'
    };

    const viewTabsStyle = {
        display: 'flex',
        borderBottom: '2px solid #ccc', // Border to separate tabs from content
        paddingBottom: '10px',
        marginBottom: '20px'
    };

    const viewTabStyle = {
        marginRight: '10px',
        padding: '10px',
        border: '1px solid #ccc',
        borderRadius: '5px',
        backgroundColor: '#fff', // White background for tabs
        cursor: 'pointer'
    };

    const columnsStyle = {
        display: 'flex',
        justifyContent: 'space-between'
    };

    const columnStyle = {
        flex: 1,
        backgroundColor: '#f9f9f9', // Slightly darker than the page for contrast
        margin: '0 10px',
        padding: '20px',
        borderRadius: '5px',
        boxShadow: '0 2px 4px rgba(0,0,0,0.1)' // Soft shadow for depth
    };

    const taskItemStyle = {
        backgroundColor: '#fff',
        border: '1px solid #ddd',
        borderRadius: '4px',
        padding: '10px',
        margin: '10px 0',
        boxShadow: '0 1px 3px rgba(0,0,0,0.1)' // Adding a subtle shadow for depth
    };

    const taskDescriptionStyle = {
        marginTop: '5px',
        fontSize: '14px',
        color: '#666' // A lighter shade for the description to differentiate from the title
    };

    return (
        <div style={pageStyle}>
            <h1>Welcome to the Main Page</h1>
            <p>Thank you for logging in.</p>
            <div style={viewTabsStyle}>
                {views.map(view => (
                    <div key={view.id} style={viewTabStyle} onClick={() => selectView(view.id)}>
                        {view.name}
                    </div>
                ))}
            </div>
            <div style={columnsStyle}>
                {columns.map(column => (
                    <div key={column.id} style={columnStyle}>
                        <h2>{column.name}</h2>
                        <div>
                            {tasks[column.id]?.map(task => (
                                <div key={task.id} style={taskItemStyle}>
                                    <strong>{task.name}</strong>
                                    <p style={taskDescriptionStyle}>{task.description}</p>
                                </div>
                            ))}
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );

}

export default MainPage;
