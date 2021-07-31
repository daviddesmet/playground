import React from "react";
import * as Yup from "yup";
import { sentenceCase } from "change-case";
import { Form, FormikProvider, useFormik } from "formik";
import { useNavigate, useParams, Link as RouterLink } from "react-router-dom";
import { useUserContext } from "../contexts/user";
import { isEmptyUuid } from "../utils";

import { Avatar, Breadcrumbs, Card, CardContent, Container, Divider, Grid, Link, TextField, Typography, Stack, Switch } from "@material-ui/core";
import { LoadingButton } from "@material-ui/lab";
import Label from "../components/Label";
import Page from "../components/Page";

// TODO: Sync status depending on switch value
// TODO: Support changing avatar

export default function UserEdit() {
  const navigate = useNavigate();
  const { id } = useParams();

  const { actions } = useUserContext();
  const user = actions.getUser(id);

  const isNewUser = (id) => {
    return isEmptyUuid(id);
  };

  const UserSchema = Yup.object().shape({
    name: Yup.string().min(2, "Too Short!").max(100, "Too Long!").required("Name required"),
    email: Yup.string().email("Email must be a valid email address").required("Email is required"),
    project: Yup.string().min(2, "Too Short!").max(50, "Too Long!").required("Project name required"),
    role: Yup.string().min(2, "Too Short!").max(50, "Too Long!").required("Role name required"),
  });

  const formik = useFormik({
    initialValues: {
      avatarUrl: user?.avatarUrl || "",
      name: user?.name || "",
      email: user?.email || "",
      project: user?.project || "",
      role: user?.role || "",
      status: user?.status || "unknown",
      sloth: user?.status === "lazy",
    },
    validationSchema: UserSchema,
    onSubmit: () => {
      // Fake a saving delay...
      setTimeout(() => {
        values.id = id;
        values.status = values.sloth ? "lazy" : "active";
        // delete values.sloth;
        // console.log(JSON.stringify(values, null, 2));
        actions.saveUser(values);
        // We need to know if it was saved before moving on but meh...
        navigate("/dashboard/users", { replace: true });
      }, 400);
    },
  });

  const { errors, touched, values, handleSubmit, isSubmitting, getFieldProps } = formik;

  return (
    <Page title="User | Definity First University">
      <Container>
        <Stack direction="column" alignItems="flex-start" justifyContent="space-between" mb={5}>
          <Typography variant="h4" gutterBottom>
            {isNewUser(id) ? "New" : "Edit"} User
          </Typography>
          <Breadcrumbs aria-label="breadcrumb">
            <Link underline="hover" color="inherit" component={RouterLink} to="/">
              Home
            </Link>
            <Link underline="hover" color="inherit" component={RouterLink} to="/dashboard/users">
              Users
            </Link>
            <Typography color="text.primary">{isNewUser(id) ? "New" : "Edit"} User</Typography>
          </Breadcrumbs>
        </Stack>
        <FormikProvider value={formik}>
          <Form autoComplete="off" noValidate onSubmit={handleSubmit}>
            <Grid container spacing={3}>
              <Grid item xs={4}>
                <Card>
                  <Stack margin={3}>
                    <Stack direction="row" justifyContent="flex-end" alignItems="baseline" spacing={2}>
                      <Label variant="ghost" color={values.status === "lazy" ? "error" : values.status === "active" ? "success" : "default"}>
                        {sentenceCase(values.status)}
                      </Label>
                    </Stack>
                    <Stack justifyContent="center" alignItems="center" mt={4}>
                      <Avatar sx={{ width: 100, height: 100 }} src={values.avatarUrl}></Avatar>
                    </Stack>
                  </Stack>
                  <Stack direction="row" justifyContent="space-between" alignItems="center" spacing={2} margin={3}>
                    <Typography variant="body2">Is the user blazing fast as a sloth?</Typography>
                    <Switch color="primary" {...getFieldProps("sloth")} checked={values.sloth} inputProps={{ "aria-label": "controlled" }} />
                  </Stack>
                </Card>
              </Grid>
              <Grid item xs={8}>
                <Card>
                  <CardContent>
                    <Stack spacing={3}>
                      <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                        <TextField
                          fullWidth
                          label="Full name"
                          {...getFieldProps("name")}
                          error={Boolean(touched.name && errors.name)}
                          helperText={touched.name && errors.name}
                        />

                        <TextField
                          fullWidth
                          autoComplete="username"
                          type="email"
                          label="Email address"
                          {...getFieldProps("email")}
                          error={Boolean(touched.email && errors.email)}
                          helperText={touched.email && errors.email}
                        />
                      </Stack>
                      <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                        <TextField
                          fullWidth
                          label="Project"
                          {...getFieldProps("project")}
                          error={Boolean(touched.project && errors.project)}
                          helperText={touched.project && errors.project}
                        />

                        <TextField
                          fullWidth
                          label="Role"
                          {...getFieldProps("role")}
                          error={Boolean(touched.role && errors.role)}
                          helperText={touched.role && errors.role}
                        />
                      </Stack>
                      <Divider variant="middle" />
                      <Stack direction="row" justifyContent="flex-end" alignItems="baseline" spacing={2}>
                        <LoadingButton size="medium" type="submit" variant="contained" loading={isSubmitting}>
                          Save Changes
                        </LoadingButton>
                      </Stack>
                    </Stack>
                  </CardContent>
                </Card>
              </Grid>
            </Grid>
          </Form>
        </FormikProvider>
      </Container>
    </Page>
  );
}
