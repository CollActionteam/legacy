import React, { useState } from "react";
import { Section } from "../../components/Section";
import { Alert } from "../../components/Alert";
import { Grid, FormGroup, TextField, Button } from "@material-ui/core";
import { gql, useMutation } from "@apollo/client";
import styles from "./ResetPassword.module.scss";

const ResetPasswordPage = () => {
    const searchParams = new URLSearchParams(window.location.search);
    const resetCode = searchParams.get("code");
    const email = searchParams.get("email");
    const [ password, setPassword ] = useState("");
    const [ confirmPassword, setConfirmPassword ] = useState("");
    const [ errorMessage, setErrorMessage ] = useState<string | null>(null);
    const [ infoMessage, setInfoMessage ] = useState<string | null>(null);
    const valid = resetCode !== null && email !== null && password !== confirmPassword;
    const [ resetPassword ] = useMutation(
        FORGOT_PASSWORD,
        {
            variables: {
                email: email,
                code: resetCode,
                password: password
            },
            onCompleted: (data) => {
                if (data.applicationUser.resetPassword.succeeded) {
                    setErrorMessage(null);
                    setInfoMessage("Your password has been reset. You can log in with your new password now.");
                } else {
                    let error = data.applicationUser.resetPassword.errors.map((e: any) => e.description).join(", ");
                    setErrorMessage(error);
                }
            },
            onError: (data) => {
                setErrorMessage(data.message);
            }
        }
    );

    return <React.Fragment>
        <Section className={styles.intro}>
            <h1 className={styles.title}>Reset Password</h1>
        </Section>
        {
            errorMessage ? <Alert type="error" text={errorMessage} /> : null
        }
        {
            infoMessage ? <Alert type="info" text={infoMessage} /> : null
        }
        <Section color="grey">
            <Grid container justify="center">
                <Grid item sm={6}>
                    <FormGroup>
                        <TextField onChange={(action) => setPassword(action.target.value)} value={password} required label="Password" type="password" />
                        <TextField onChange={(action) => setConfirmPassword(action.target.value)} value={confirmPassword} error={password !== confirmPassword} required label="Confirm Password" type="password" />
                        <Button disabled={!valid} onClick={() => resetPassword()}>Reset my password</Button>
                    </FormGroup>
                </Grid>
            </Grid>
        </Section>
    </React.Fragment>;
};

export default ResetPasswordPage;

const FORGOT_PASSWORD = gql`
    mutation ResetPassword($email: String!, $code: String!, $password: String!) {
        applicationUser {
            resetPassword(email: $email, code: $code, password: $password) {
                succeeded
                errors {
                    code
                    description
                }
            }
        }
    }
`;