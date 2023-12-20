import React, { useState, useEffect } from 'react';
import axios from 'axios';
import leftArrow from '../pictures/arrowLeft.png';
import rightArrow from '../pictures/arrowRight.png';
import crossMark from '../pictures/cross.png';
import editIcon from '../pictures/editIcon.png';
import Modal from 'react-modal';

function MainPage() {
    const [views, setViews] = useState([]);
    const [selectedView, setSelectedView] = useState(null);
    const [columns, setColumns] = useState([]);
    const [tasks, setTasks] = useState({});
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [newTaskName, setNewTaskName] = useState('');
    const [newTaskDescription, setNewTaskDescription] = useState('');
    const [creatingColumnId, setCreatingColumnId] = useState(null);
    const [newViewName, setNewViewName] = useState('');
    const [newColumnName, setNewColumnName] = useState('');
    const [modalType, setModalType] = useState('');
    const [newViewDescription, setNewViewDescription] = useState('');
    const [newColumnDescription, setNewColumnDescription] = useState('');
    const [editingTaskId, setEditingTaskId] = useState(null);
    const [editingTask, setEditingTask] = useState(null);


    const authAxios = axios.create({
        headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
        }
    });

    const openModal = (type, columnId = null) => {
        setModalType(type);
        if (type === 'task') {
            setCreatingColumnId(columnId);
        }
        setIsModalOpen(true);
    };

    const openEditModal = (task) => {
        setEditingTask(task);
        setModalType('editTask');
        setIsModalOpen(true);
    };


    const closeModal = () => {
        setIsModalOpen(false);
        setNewTaskName('');
        setNewTaskDescription('');
        setCreatingColumnId(null);
        setNewViewName('');
        setNewViewDescription('');
        setNewColumnName('');
    };

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

    const updateTask = async () => {
        if (!editingTask || !editingTask.name.trim() || !editingTask.description.trim()) return;

        let columnId = null;
        for (const colId in tasks) {
            if (tasks[colId].some(task => task.id === editingTask.id)) {
                columnId = colId;
                break;
            }
        }

        if (!columnId) {
            console.error('Column ID could not be found for the task.');
            return;
        }

        try {
            const updatedTask = {
                Name: editingTask.name,
                Description: editingTask.description,
            };

            const url = `https://localhost:7229/api/views/${selectedView}/columns/${columnId}/tasks/${editingTask.id}`;
            const response = await authAxios.put(url, updatedTask);

            setTasks(prevTasks => {
                const newTasks = { ...prevTasks };
                newTasks[columnId] = newTasks[columnId].map(task =>
                    task.id === editingTask.id ? { ...task, ...response.data } : task
                );
                return newTasks;
            });

            setEditingTask(null);
            closeModal();
        } catch (error) {
            console.error('Failed to update task:', error);
        }
    };

    const createView = async () => {
        if (!newViewName.trim()) {
            console.error('View name is required');
            return;
        }

        try {
            const viewData = {
                Name: newViewName,
                Description: newViewDescription
            };
            const response = await authAxios.post('https://localhost:7229/api/views', viewData);
            setViews([...views, response.data]);
            closeModal();
        } catch (error) {
            console.error('Failed to create view:', error);
        }
    };

    const deleteColumn = async (columnId) => {
        if (!selectedView) return;

        try {
            await authAxios.delete(`https://localhost:7229/api/views/${selectedView}/columns/${columnId}`);

            setColumns(currentColumns => currentColumns.filter(column => column.id !== columnId));
        } catch (error) {
            console.error('Failed to delete column:', error.response.data);
        }
    };

    const deleteView = async (viewId) => {
        try {
            await authAxios.delete(`https://localhost:7229/api/views/${viewId}`);

            setViews(currentViews => currentViews.filter(view => view.id !== viewId));

            if (selectedView === viewId) {
                setSelectedView(null);
                setColumns([]);
                setTasks({});
            }
        } catch (error) {
            console.error('Failed to delete view:', error.response.data);
        }
    };

    const createColumn = async () => {
        if (!selectedView || !newColumnName.trim()) {
            console.error('Column name is required');
            return;
        }

        try {
            const columnData = {
                Name: newColumnName,
                Description: newColumnDescription,
            };
            console.log('Creating column with data:', columnData);

            const response = await authAxios.post(`https://localhost:7229/api/views/${selectedView}/columns`, columnData);
            setColumns([...columns, response.data]);
            setNewColumnName('');
            setNewColumnDescription('');
        } catch (error) {
            console.error('Failed to create column:', error.response.data);
        }
    };


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

    const createTask = async () => {
        if (!creatingColumnId) return;

        try {
            const response = await authAxios.post(`https://localhost:7229/api/views/${selectedView}/columns/${creatingColumnId}/tasks`, {
                Name: newTaskName,
                Description: newTaskDescription
            });

            setTasks((prevTasks) => ({
                ...prevTasks,
                [creatingColumnId]: [...(prevTasks[creatingColumnId] || []), response.data]
            }));

            closeModal();
        } catch (error) {
            console.error('Failed to create task:', error);
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

            return newTasks;
        });

        try {
            await authAxios.delete(`https://localhost:7229/api/views/${selectedView}/columns/${sourceColumnId}/tasks/${taskId}`);

            const response = await authAxios.post(`https://localhost:7229/api/views/${selectedView}/columns/${destinationColumnId}/tasks`, {
                Name: taskToMove.name,
                Description: taskToMove.description
            });

            setTasks(prevTasks => {
                const newTasks = { ...prevTasks };
                const newTaskWithId = { ...taskToMove, id: response.data.id };
                newTasks[destinationColumnId].push(newTaskWithId);
                return newTasks;
            });
        } catch (error) {
            console.error('Failed to move task:', error);
            setTasks(prevTasks => {
                return prevTasks;
            });
        }
    };

    const customModalStyles = {
        content: {
            top: '50%',
            left: '50%',
            right: 'auto',
            bottom: 'auto',
            marginRight: '-50%',
            transform: 'translate(-50%, -50%)',
            width: '400px',
            padding: '20px',
            borderRadius: '10px',
            backgroundColor: '#fff'
        },
    };

    const inputStyle = {
        width: '100%',
        padding: '10px',
        marginBottom: '10px',
        borderRadius: '4px',
        border: '1px solid #ccc'
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
        marginBottom: '20px',
        position: 'relative',
        width: 'auto',

    };

    const viewDetails = {
        cursor: 'pointer',
    };

    const viewTabStyle = {
        marginRight: '10px',
        padding: '10px',
        border: '1px solid #ccc',
        borderRadius: '5px',
        backgroundColor: '#fff',
        cursor: 'pointer',
        position: 'relative',
        width: 'auto',
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
    };

    const columnsStyle = {
        display: 'flex',
        justifyContent: 'space-between',
        position: 'relative'
    };

    const columnStyle = {
        flex: 1,
        backgroundColor: '#f9f9f9',
        margin: '0 10px',
        padding: '20px',
        borderRadius: '5px',
        boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
        position: 'relative'
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

    const arrowStyle = {
        border: 'none',
        background: 'transparent',
        width: '15px',
        height: '15px',
        cursor: 'pointer'
    };

    const buttonStyle = {
        backgroundColor: '#4CAF50',
        color: 'white',
        padding: '10px 20px',
        border: 'none',
        borderRadius: '5px',
        cursor: 'pointer',
        outline: 'none',
        fontSize: '16px',
        margin: '5px',
        transition: 'background-color 0.3s',
    };

    const viewNameContainerStyle = {
        display: 'flex',
        alignItems: 'center',
        position: 'relative',
        cursor: 'pointer'
    };

    const addButtonStyle = {
        background: 'transparent',
        border: 'none',
        cursor: 'pointer',
        fontSize: '20px',
    };


    return (
        <div style={pageStyle}>
            <h1>Welcome to the Main Page</h1>
            <p>Thank you for logging in.</p>
            <div style={viewTabsStyle}>
                {views.map(view => (
                    <div key={view.id} style={viewTabStyle} onClick={() => selectView(view.id)}>
                        <div style={viewDetails}>
                            <strong>{view.name}</strong>
                            <p>{view.description}</p>
                        </div>
                        <button
                            onClick={(e) => {
                                e.stopPropagation();
                                deleteView(view.id);
                            }}
                            style={deleteButtonStyle }>
                            <img src={crossMark} alt="Delete View" style={{ width: '15px', height: '15px' }} />
                        </button>
                    </div>
                ))}
                <button onClick={() => openModal('view')} style={buttonStyle} >Create View</button>
            </div>
            <button onClick={() => openModal('column')} style={{ ...buttonStyle, alignSelf: 'flex-start' }}>Create Column</button>
            <div style={columnsStyle}>
                {columns.map((column, columnIndex) => (
                    <div key={column.id} style={columnStyle}>
                        <h2>{column.name}</h2>
                        <button onClick={() => deleteColumn(column.id)} style={deleteButtonStyle}>
                            <img src={crossMark} alt="Delete Column" style={{ width: '15px', height: '15px' }} />
                        </button>
                        <button onClick={() => openModal('task', column.id)} style={addButtonStyle }>+</button>
                        <div>
                            {tasks[column.id]?.map((task, taskIndex) => (
                                <div key={task.id} style={taskItemStyle}>
                                    <strong>{task.name}</strong>
                                    <p style={taskDescriptionStyle}>{task.description}</p>
                                    <button onClick={() => deleteTask(task.id, column.id)} style={deleteButtonStyle}>
                                        <img src={crossMark} className="cross-mark" alt="Delete" style={{ width: '15px', height: '15px' }} />
                                    </button>
                                    <button style={{ ...deleteButtonStyle, right: '30px' }} onClick={() => openEditModal(task)}>
                                        <img src={editIcon} className="edit-icon" alt="Edit" style={{ width: '15px', height: '15px' }} />
                                    </button>
                                    {columnIndex > 0 && (
                                        <button onClick={() => moveTask(task.id, column.id, columns[columnIndex - 1].id)} style={arrowStyle}>
                                            <img src={leftArrow} className="arrow-left" alt="Move Left" style={{ width: '15px', height: '15px' }} />
                                        </button>
                                    )}
                                    {columnIndex < columns.length - 1 && (
                                        <button onClick={() => moveTask(task.id, column.id, columns[columnIndex + 1].id)} style={arrowStyle}>
                                            <img src={rightArrow} className="arrow-right" alt="Move Right" style={{ width: '15px', height: '15px' }} />
                                        </button>
                                    )}
                                </div>
                            ))}
                        </div>
                    </div>
                ))}
            </div>
            <Modal isOpen={isModalOpen} onRequestClose={closeModal} contentLabel={modalType === 'editTask' ? "Edit Task" : "Create Item"} style={customModalStyles}>
                <h2>{modalType === 'editTask' ? "Edit Task" : `Create ${modalType.charAt(0).toUpperCase() + modalType.slice(1)}`}</h2>

                {modalType === 'editTask' && editingTask ? (
                    <>
                        <input
                            type="text"
                            placeholder="Name"
                            value={editingTask ? editingTask.name : ''}
                            onChange={(e) => setEditingTask({ ...editingTask, name: e.target.value })}
                            style={inputStyle}
                        />
                        <textarea
                            placeholder="Description"
                            value={editingTask ? editingTask.description : ''}
                            onChange={(e) => setEditingTask({ ...editingTask, description: e.target.value })}
                            style={inputStyle}
                        />
                        <button onClick={() => updateTask(editingTask)}>Update Task</button>
                    </>
                ) : (
                    <>
                        <input
                            type="text"
                            placeholder="Name"
                            value={modalType === 'task' ? newTaskName : (modalType === 'view' ? newViewName : newColumnName)}
                            onChange={(e) => modalType === 'task' ? setNewTaskName(e.target.value) : (modalType === 'view' ? setNewViewName(e.target.value) : setNewColumnName(e.target.value))}
                            style={inputStyle}
                        />
                        <textarea
                            placeholder="Description"
                            value={modalType === 'task' ? newTaskDescription : (modalType === 'view' ? newViewDescription : newColumnDescription)}
                            onChange={(e) => modalType === 'task' ? setNewTaskDescription(e.target.value) : (modalType === 'view' ? setNewViewDescription(e.target.value) : setNewColumnDescription(e.target.value))}
                            style={inputStyle}
                        />
                        <button onClick={modalType === 'task' ? createTask : (modalType === 'view' ? createView : createColumn)}>Create</button>
                    </>
                )}

                <button onClick={closeModal}>Cancel</button>
            </Modal>
        </div>
    );

}

export default MainPage;
