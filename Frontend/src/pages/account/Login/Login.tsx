import React from "react";
import { Button } from "../../../components/Button/Button";
import { Section } from "../../../components/Section/Section";
import { Alert } from "../../../components/Alert/Alert";
import { Link, useLocation } from "react-router-dom";
import styles from "./Login.module.scss";

import {
  Grid,
  TextField,
  FormControl,
  FormControlLabel,
  Checkbox,
  FormGroup,
} from "@material-ui/core";
import { useSettings } from "../../../providers/SettingsProvider";
import { Helmet } from "react-helmet";

const LoginPage = () => {
  const actionLogin = `${process.env.REACT_APP_BACKEND_URL}/account/login`;
  const actionExternalLogin = `${process.env.REACT_APP_BACKEND_URL}/account/externalLogin`;
  
  const errorUrl = `${window.location.origin}/account/login`;
  const searchParams = new URLSearchParams(useLocation().search);

  const returnUrlQuery = searchParams.get("returnUrl") ?? "";  
  const returnUrl = `${!returnUrlQuery.startsWith('http') ? window.location.origin : ""}${returnUrlQuery}`;
  
  const errorType = searchParams.get("error");
  const errorMessage = searchParams.get("message");

  const { externalLoginProviders } = useSettings();
  if (errorType && errorMessage)
  {
    console.error({ errorType, errorMessage });
  }

  return (
    <>
      <Helmet>
        <title>Login</title>
        <meta name="description" content="Login" />
      </Helmet>
      <Section className={styles.intro}>
        <h1 className={styles.title}>Login</h1>
        <h2 className={styles.subtitle}>
          (Use a local account to log in )
        </h2>
      </Section>
      <Alert type="error" text={errorMessage} />
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
                { externalLoginProviders.map(
                  (provider: string) => (
                    <FormControl key={provider} margin="normal">
                      <Button name="provider" value={provider}>
                        Login with {provider}
                      </Button>
                    </FormControl>
                  )
                ) }
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
                <Link to="/account/register-user">Register as new user</Link>
                <Link to="/account/forgot-password">I forgot my password</Link>
              </FormGroup>
            </form>
          </Grid>
        </Grid>
      </Section>
  </>
  );
}

export default LoginPage;