import React, { createContext, useContext, useEffect, useState } from "react";
import { getTasks, getTaskById, setSelectedTasks, saveTask, deleteTasks, deleteTaskById } from "./actions";

const TaskContext = createContext();

function TaskProvider({ children }) {
  const [state, setState] = useState({
    taskList: [],
    selectedTasks: [],
  });

  useEffect(() => {
    (function () {
      const taskList = getTasks();
      setState((currentState) => ({ ...currentState, taskList }));
    })();
  }, []);

  // context actions
  const handleGetTaskById = (userId) => {
    return getTaskById(state, userId);
  };

  const handleSaveTask = (user) => {
    saveTask(setState, user);
  };

  const handleSelectedTasks = (userIds) => {
    setSelectedTasks(setState, userIds);
  };

  const handleDeleteSelectedTasks = () => {
    deleteTasks(setState);
  };

  const handleDeleteTaskById = (id) => {
    deleteTaskById(setState, id);
  };

  return (
    <TaskContext.Provider
      value={{
        state,
        actions: {
          getTask: handleGetTaskById,
          setSelectedTasks: handleSelectedTasks,
          saveTask: handleSaveTask,
          deleteSelectedTasks: handleDeleteSelectedTasks,
          deleteTaskById: handleDeleteTaskById,
        },
      }}
    >
      {children}
    </TaskContext.Provider>
  );
}

export function useTaskContext() {
  return useContext(TaskContext);
}

export default TaskProvider;
