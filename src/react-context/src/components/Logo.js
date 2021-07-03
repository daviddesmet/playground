import PropTypes from "prop-types";

import { Box } from "@material-ui/core";

Logo.propTypes = {
  sx: PropTypes.object
};

export default function Logo({ sx }) {
  return (
    <Box
      component="img"
      src="https://www.definityfirst.com/hubfs/DefinityFirst-Logo-01-01.svg"
      sx={{ width: 160, height: 30, ...sx }}
    />
  );
}
