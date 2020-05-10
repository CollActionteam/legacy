import React, { useState } from "react";
import { Section } from "../../../components/Section/Section";
import { Alert } from "../../../components/Alert/Alert";
import { Grid, FormGroup, TextField } from "@material-ui/core";
import { gql, useMutation } from "@apollo/client";
import styles from "./ForgotPassword.module.scss";
import { Button } from "../../../components/Button/Button";
import { Helmet } from "react-helmet";

const ForgotPasswordPage = () => {
    const [ errorMessage, setErrorMessage ] = useState<string | null>(null);
    const [ infoMessage, setInfoMessage ] = useState<string | null>(null);
    const [ email, setEmail ] = useState("");
    const [ forgotPassword ] = useMutation(
        FORGOT_PASSWORD,
        {
            variables: {
                email: email
            },
            onCompleted: (data) => {
                if (data.user.forgotPassword.succeeded) {
                    setErrorMessage(null);
                    setInfoMessage("An e-mail has been sent, click on the link in that e-mail to reset your password");
                } else {
                    let error = data.user.forgotPassword.errors.map((e: any) => e.description).join(", ");
                    setErrorMessage(error);
                }
            },
            onError: (data) => {
                setErrorMessage(data.message);
                console.error(data.message);
            }
        }
    );
    return <>
        <Helmet>
            <title>Forgot Password CollAction</title>
            <meta name="description" content="Forgot Password" />
        </Helmet>
        <Section className={styles.intro}>
            <h1 className={styles.title}>Forgot Password</h1>
        </Section>
        <Section color="grey">
            <Grid container justify="center">
                <Grid item sm={6}>
                    <Alert type="error" text={errorMessage} />
                    <Alert type="info" text={infoMessage} />
                    <FormGroup>
                        <TextField margin="normal" onChange={(action) => setEmail(action.target.value)} value={email} required label="E-Mail Address" type="email" />
                        <Button onClick={() => forgotPassword()}>Reset my password</Button>
                    </FormGroup>
                </Grid>
            </Grid>
        </Section>
    </>;
};

export default ForgotPasswordPage;

const FORGOT_PASSWORD = gql`
    mutation ForgotPassword($email: String!) {
        user {
            forgotPassword(email: $email) {
                succeeded
                errors {
                    code
                    description
                }
            }
        }
    }
`;