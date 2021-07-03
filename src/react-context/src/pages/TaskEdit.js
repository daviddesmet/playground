import * as Yup from "yup";
import { Form, FormikProvider, useFormik } from "formik";
import { useNavigate, useParams, Link as RouterLink } from "react-router-dom";
import { useUserContext } from "../contexts/user";
import { useTaskContext } from "../contexts/task";
import { isEmptyUuid } from "../utils";

import {
  Autocomplete,
  Breadcrumbs,
  Card,
  CardContent,
  Container,
  Divider,
  Link,
  TextField,
  Typography,
  Stack,
  Switch
} from "@material-ui/core";
import { LoadingButton } from "@material-ui/lab";
import Page from "../components/Page";

export default function TaskEdit() {
  const navigate = useNavigate();
  const { id } = useParams();

  const { actions } = useTaskContext();
  const task = actions.getTask(id);

  const { state: userState } = useUserContext();
  const users = userState.userList;
  // const users = userState.userList.map((user) => ({
  //   label: user.name,
  //   id: user.id
  // }));
  users.sort((a, b) => (a.label > b.label ? 1 : -1));

  const isNewTask = (id) => {
    return isEmptyUuid(id);
  };

  const handleUserOptionChange = (event, newValue) => {
    if (newValue) {
      const found = users.find((u) => u.id === newValue.id);
      if (found) {
        values.user = found;
      }
    } else {
      values.user = null;
    }
  };

  const TaskSchema = Yup.object().shape({
    name: Yup.string()
      .min(2, "Too Short!")
      .max(100, "Too Long!")
      .required("Task required"),
    user: Yup.object()
      .shape({
        name: Yup.string().required("User is required")
      })
      .nullable()
      .required("User is required")
  });

  const formik = useFormik({
    initialValues: {
      name: task?.name || "",
      user: task?.user || "",
      completed: task?.completed || false
    },
    validationSchema: TaskSchema,
    onSubmit: () => {
      // Fake a saving delay...
      setTimeout(() => {
        values.id = id;
        // console.log(JSON.stringify(values, null, 2));
        actions.saveTask(values);
        // We need to know if it was saved before moving on but meh...
        navigate("/dashboard/tasks", { replace: true });
      }, 400);
    }
  });

  const {
    errors,
    touched,
    values,
    handleSubmit,
    isSubmitting,
    getFieldProps
  } = formik;

  return (
    <Page title="Task | Definity First University">
      <Container>
        <Stack
          direction="column"
          alignItems="flex-start"
          justifyContent="space-between"
          mb={5}
        >
          <Typography variant="h4" gutterBottom>
            {isNewTask(id) ? "New" : "Edit"} Task
          </Typography>
          <Breadcrumbs aria-label="breadcrumb">
            <Link
              underline="hover"
              color="inherit"
              component={RouterLink}
              to="/"
            >
              Home
            </Link>
            <Link
              underline="hover"
              color="inherit"
              component={RouterLink}
              to="/dashboard/users"
            >
              Tasks
            </Link>
            <Typography color="text.primary">
              {isNewTask(id) ? "New" : "Edit"} Task
            </Typography>
          </Breadcrumbs>
        </Stack>
        <FormikProvider value={formik}>
          <Form autoComplete="off" noValidate onSubmit={handleSubmit}>
            <Card>
              <CardContent>
                <Stack spacing={3}>
                  <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                    <TextField
                      fullWidth
                      label="Task"
                      {...getFieldProps("name")}
                      error={Boolean(touched.name && errors.name)}
                      helperText={touched.name && errors.name}
                    />
                  </Stack>
                  <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                    <Autocomplete
                      fullWidth
                      {...getFieldProps("user")}
                      getOptionLabel={(option) => option.name}
                      isOptionEqualToValue={(option, value) =>
                        value === "" ? true : option.id === value?.id
                      }
                      options={users}
                      onChange={handleUserOptionChange}
                      value={values.user || null}
                      renderInput={(params) => (
                        <TextField
                          {...params}
                          label="User"
                          error={Boolean(touched.user && errors.user)}
                          helperText={touched.user && errors.user}
                        />
                      )}
                    />
                    <Stack
                      direction="row"
                      justifyContent="space-between"
                      alignItems="center"
                      spacing={2}
                      margin={3}
                      pl={6}
                    >
                      <Typography variant="body2">Completed</Typography>
                      <Switch
                        color="primary"
                        {...getFieldProps("completed")}
                        checked={values.completed}
                        inputProps={{ "aria-label": "controlled" }}
                      />
                    </Stack>
                  </Stack>
                  <Divider variant="middle" />
                  <Stack
                    direction="row"
                    justifyContent="flex-end"
                    alignItems="baseline"
                    spacing={2}
                  >
                    <LoadingButton
                      size="medium"
                      type="submit"
                      variant="contained"
                      loading={isSubmitting}
                    >
                      Save Changes
                    </LoadingButton>
                  </Stack>
                </Stack>
              </CardContent>
            </Card>
          </Form>
        </FormikProvider>
      </Container>
    </Page>
  );
}
