import PropTypes from "prop-types";
import { Form, FormikProvider, useFormik } from "formik";
import { useTaskContext } from "../../../contexts/task";
import ACCOUNT from "../../../_mocks_/account";

import {
  Box,
  Card,
  Checkbox,
  CardHeader,
  Typography,
  FormControlLabel,
  Stack
} from "@material-ui/core";

TaskItem.propTypes = {
  task: PropTypes.object,
  checked: PropTypes.bool,
  onChange: PropTypes.func,
  formik: PropTypes.object
};

function TaskItem({ task, checked, onChange, formik, ...other }) {
  const { getFieldProps } = formik;

  return (
    <Stack direction="row" justifyContent="space-between" sx={{ py: 0.75 }}>
      <FormControlLabel
        control={
          <Checkbox
            {...getFieldProps("checked")}
            value={task.id}
            checked={checked}
            onChange={onChange}
            inputProps={{ "aria-label": "controlled" }}
            {...other}
          />
        }
        label={
          <Typography
            variant="body2"
            sx={{
              ...(checked && {
                color: "text.disabled",
                textDecoration: "line-through"
              })
            }}
          >
            {task.name}
          </Typography>
        }
      />
    </Stack>
  );
}

export default function AppTasks() {
  const { state, actions } = useTaskContext();
  const tasks = state.taskList.filter((task) => task.user.id === ACCOUNT.id);

  const handleOnChange = (e) => {
    let newList = values.checked;
    if (e.target.checked) {
      newList.push(e.target.value);
    } else {
      newList = values.checked.filter((id) => id !== e.target.value);
    }
    setFieldValue("checked", newList);

    const found = state.taskList.find((task) => task.id === e.target.value);
    if (found) {
      found.completed = e.target.checked;
      actions.saveTask(found);
    }
  };

  const formik = useFormik({
    initialValues: {
      checked: tasks.filter((t) => t.completed).map((t) => t.id)
    },
    onSubmit: (values) => {
      console.log("submit", values);
    }
  });

  const { values, setFieldValue, handleSubmit } = formik;

  return (
    <Card>
      <CardHeader title="My Tasks" />
      <Box sx={{ px: 3, py: 1 }}>
        <FormikProvider value={formik}>
          <Form autoComplete="off" noValidate onSubmit={handleSubmit}>
            {/* {TASKS.map((task) => (
              <TaskItem
                key={task}
                task={task}
                formik={formik}
                checked={values.checked.includes(task)}
              />
            ))} */}
            {tasks.map((task) => (
              <TaskItem
                key={task.id}
                task={task}
                formik={formik}
                checked={values.checked.includes(task.id)}
                // checked={task.completed}
                onChange={handleOnChange}
              />
            ))}
          </Form>
        </FormikProvider>
      </Box>
    </Card>
  );
}
