import React, { useState, useEffect } from 'react';
import axios from 'axios';
import leftArrow from '../pictures/arrowLeft.png';
import rightArrow from '../pictures/arrowRight.png';
import crossMark from '../pictures/cross.png';

function MainPage() {
    const [views, setViews] = useState([]);
    const [selectedView, setSelectedView] = useState(null);
    const [columns, setColumns] = useState([]);
    const [tasks, setTasks] = useState({});

    const authAxios = axios.create({
        headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
        }
    });

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
    }, []);

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

    const deleteTask = async (taskId, columnId) => {
        try {
            await authAxios.delete(`https://localhost:7229/api/views/${selectedView}/columns/${columnId}/tasks/${taskId}`);

            setTasks(prevTasks => {
                const newTasks = { ...prevTasks };
                newTasks[columnId] = newTasks[columnId].filter(task => task.id !== taskId);
                return newTasks;
            });
        } catch (error) {
            console.error('Failed to delete task:', error);
        }
    };

    const moveTask = async (taskId, sourceColumnId, destinationColumnId) => {
        const taskToMove = tasks[sourceColumnId].find(task => task.id === taskId);
        if (!taskToMove) return;

        setTasks(prevTasks => {
            const newTasks = { ...prevTasks };

            newTasks[sourceColumnId] = newTasks[sourceColumnId].filter(task => task.id !== taskId);

            if (!newTasks[destinationColumnId]) {
                newTasks[destinationColumnId] = [];
            }
            newTasks[destinationColumnId].push(taskToMove);
            return newTasks;
        });

        try {
            await authAxios.delete(`https://localhost:7229/api/views/${selectedView}/columns/${sourceColumnId}/tasks/${taskId}`);
            await authAxios.post(`https://localhost:7229/api/views/${selectedView}/columns/${destinationColumnId}/tasks`, {
                Name: taskToMove.name,
                Description: taskToMove.description
            });
        } catch (error) {
            console.error('Failed to move task:', error);
            setTasks(prevTasks => {
                return prevTasks;
            });
        }
    };

    const pageStyle = {
        backgroundColor: '#f0f0f0',
        minHeight: '100vh',
        color: '#333',
        padding: '20px'
    };

    const viewTabsStyle = {
        display: 'flex',
        borderBottom: '2px solid #ccc',
        paddingBottom: '10px',
        marginBottom: '20px'
    };

    const viewTabStyle = {
        marginRight: '10px',
        padding: '10px',
        border: '1px solid #ccc',
        borderRadius: '5px',
        backgroundColor: '#fff',
        cursor: 'pointer'
    };

    const columnsStyle = {
        display: 'flex',
        justifyContent: 'space-between'
    };

    const columnStyle = {
        flex: 1,
        backgroundColor: '#f9f9f9',
        margin: '0 10px',
        padding: '20px',
        borderRadius: '5px',
        boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
    };

    const taskItemStyle = {
        backgroundColor: '#fff',
        border: '1px solid #ddd',
        borderRadius: '4px',
        padding: '10px',
        margin: '10px 0',
        boxShadow: '0 1px 3px rgba(0,0,0,0.1)',
         position: 'relative'
    };

    const taskDescriptionStyle = {
        marginTop: '5px',
        fontSize: '14px',
        color: '#666'
    };

    const deleteButtonStyle = {
        position: 'absolute',
        top: '5px',
        right: '5px',
        border: 'none',
        background: 'transparent',
        padding: '5px',
        cursor: 'pointer',
        outline: 'none'
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
                {columns.map((column, columnIndex) => (
                    <div key={column.id} style={columnStyle}>
                        <h2>{column.name}</h2>
                        <div>
                            {tasks[column.id]?.map((task, taskIndex) => (
                                <div key={task.id} style={taskItemStyle}>
                                    <strong>{task.name}</strong>
                                    <p style={taskDescriptionStyle}>{task.description}</p>
                                    <button onClick={() => deleteTask(task.id, column.id)} style={deleteButtonStyle}>
                                        <img src={crossMark} className="cross-mark" alt="Delete" style={{ width: '25px', height: '25px' }} />
                                    </button>
                                    {columnIndex > 0 && (
                                        <button onClick={() => moveTask(task.id, column.id, columns[columnIndex - 1].id)}>
                                            <img src={leftArrow} className="arrow-left" alt="Move Left" style={{ width: '25px', height: '25px' }} />
                                        </button>
                                    )}
                                    {columnIndex < columns.length - 1 && (
                                        <button onClick={() => moveTask(task.id, column.id, columns[columnIndex + 1].id)}>
                                            <img src={rightArrow} className="arrow-right" alt="Move Right" style={{ width: '25px', height: '25px' }} />
                                        </button>
                                    )}
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
