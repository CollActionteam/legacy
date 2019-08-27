import React from "react";
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import { Link } from "gatsby";
import logo from "../../../static/assets/logo.svg";
import styles from "./style.module.scss";

export default () => (
  <div className={styles.footer}>
    <Container>
      <Link to="/">
        <img alt="CollAction" className={styles.logo} src={logo}></img>
      </Link>
      <Grid container spacing={5}>
        <Grid item sm={4}>
          <p>
            Any questions, comments, or would you like to work together? Awesome! Email us at <Link to="mailto:collactionteam@gmail.com">collactionteam@gmail.com</Link>.<br />
            Would you like to get an occasional update on CollAction and crowdacting? Sign up for our newsletter (to your right)! We will be careful with your data, see our Privacy Policy.
          </p>
        </Grid>
        <Grid item sm={5}>
          <nav>
            <ul>
              <li>
                <Link to="/">
                  Link
                </Link>
              </li>
            </ul>
          </nav>
        </Grid>
        <Grid item sm={3}>
          Hi
        </Grid>
      </Grid>
    </Container>
  </div>
);
