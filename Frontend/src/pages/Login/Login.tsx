import React from "react";
import { Button } from "../../components/Button/Button";
import { Section } from "../../components/Section";
import { Alert } from "../../components/Alert";

import {
  Grid,
  TextField,
  FormControl,
  FormControlLabel,
  Checkbox,
  FormGroup,
} from "@material-ui/core";
import styles from "./Login.module.scss";

import { siteData } from '../../api/site';

const LoginPage = () => {
  const actionLogin = `${process.env.REACT_APP_BACKEND_URL}/account/login`;
  const actionExternalLogin = `${process.env.REACT_APP_BACKEND_URL}/account/externalLogin`;
  const returnUrl = window.location.origin;
  const errorUrl = `${returnUrl}/login`;
  const searchParams = new URLSearchParams(window.location.search);
  const errorType = searchParams.get("error");
  const errorMessage = searchParams.get("message");
  if (errorType && errorMessage)
  {
    console.error({ errorType, errorMessage });
  }

  return (
    <React.Fragment>
      {
        errorMessage ? <Alert type="error" text={errorMessage} /> : null
      }
      <Section className={styles.intro}>
        <h1 className={styles.title}>Login</h1>
        <h2 className={styles.subtitle}>
          (Use a local account to log in )
        </h2>
      </Section>
      <Section color="grey">
        <Grid container justify="center">
          <Grid item sm={6}>
            <form method="post" action={actionExternalLogin}>
              <FormGroup>
                <input
                  type="hidden"
                  name="returnUrl"
                  value={returnUrl}
                />
                <input type="hidden" name="errorUrl" value={errorUrl} />
                {siteData.loginProviders.map(
                  (provider, index) => (
                    <FormControl key={index} margin="normal">
                      <Button name="provider" value={provider.name}>
                        Login with {provider.name}
                      </Button>
                    </FormControl>
                  )
                )}
              </FormGroup>
            </form>
            <div className={styles.divider}>
              <span>or</span>
            </div>
            <form method="post" action={actionLogin}>
              <FormGroup>
                <FormControl margin="normal">
                  <TextField
                    name="Email"
                    className={styles.formControl}
                    label="E-mail"
                  />
                </FormControl>
                <FormControl margin="normal">
                  <TextField
                    name="Password"
                    className={styles.formControl}
                    label="Password"
                    type="password"
                  />
                </FormControl>
                <FormControlLabel
                  control={<Checkbox color="default" />}
                  label="Remember me"
                />
                <input
                  type="hidden"
                  name="returnUrl"
                  value={returnUrl}
                />
                <input type="hidden" name="errorUrl" value={errorUrl} />
                <Button type="submit">Login</Button>
              </FormGroup>
            </form>
          </Grid>
        </Grid>
      </Section>
  </React.Fragment>
  );
}

export default LoginPage;