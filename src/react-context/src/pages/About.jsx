import React from "react";
import { motion } from "framer-motion";
import { Link as RouterLink } from "react-router-dom";

// material
import { experimentalStyled as styled } from "@material-ui/core/styles";
import { Box, Button, Typography, Container } from "@material-ui/core";

// components
import { MotionContainer, varBounceIn } from "../components/animate";
import Page from "../components/Page";

const RootStyle = styled(Page)(({ theme }) => ({
  display: "flex",
  minHeight: "100%",
  alignItems: "center",
  paddingTop: theme.spacing(15),
  paddingBottom: theme.spacing(10),
}));

export default function NotFound() {
  return (
    <RootStyle title="About | Definity First University">
      <Container>
        <MotionContainer initial="initial" open>
          <Box sx={{ maxWidth: 480, margin: "auto", textAlign: "center" }}>
            <motion.div variants={varBounceIn}>
              <Typography variant="h3" paragraph>
                Definity First University
              </Typography>
            </motion.div>
            <Typography variant="h4" paragraph>
              React Training
            </Typography>
            <Typography sx={{ color: "text.secondary" }}>
              A React project showcasing React's Context, which provides a way to pass data through the component tree without having to pass props down
              manually at every level. It features an easy-to-use UI (I hope) for managing users and tasks.
            </Typography>
            <Typography sx={{ color: "text.secondary" }} mt={2}>
              Thanks to the DF University team, the people behind and above all, the instructor which made this training possible.
            </Typography>

            <motion.div variants={varBounceIn}>
              <Box component="img" src="/static/illustrations/still_worthy.jpeg" sx={{ height: 300, mx: "auto", my: { xs: 5, sm: 10 } }} />
            </motion.div>

            <Button to="/" size="large" variant="contained" component={RouterLink}>
              Go to Home
            </Button>
          </Box>
        </MotionContainer>
      </Container>
    </RootStyle>
  );
}
