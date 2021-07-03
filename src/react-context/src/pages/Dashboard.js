import { Box, Container, Grid, Typography } from "@material-ui/core";
import Page from "../components/Page";
import { AppTasks } from "../components/_dashboard/app";

import account from "../_mocks_/account";

export default function DashboardApp() {
  return (
    <Page title="Dashboard | Definity First University">
      <Container maxWidth="xl">
        <Box sx={{ pb: 5 }}>
          <Typography variant="h4">Welcome back {account.firstName}</Typography>
        </Box>
        <Grid container spacing={3}>
          <Grid item xs={12} sm={12} md={12}>
            <AppTasks />
          </Grid>
        </Grid>
      </Container>
    </Page>
  );
}
