import React, { useState } from "react";
import { Section } from "../../components/Section";
import { Alert } from "../../components/Alert";
import { Grid, Checkbox, FormGroup, TextField, Button, FormControlLabel, FormControl, FormHelperText } from "@material-ui/core";
import { gql, useMutation } from "@apollo/client";
import styles from "./RegisterUser.module.scss";
import { Link } from "react-router-dom";

const RegisterUserPage = () => {
    const [ errorMessage, setErrorMessage ] = useState<string | null>(null);
    const [ infoMessage, setInfoMessage ] = useState<string | null>(null);
    const [ firstName, setFirstName ] = useState("");
    const [ lastName, setLastName ] = useState("");
    const [ email, setEmail ] = useState("");
    const [ confirmEmail, setConfirmEmail ] = useState("");
    const [ password, setPassword ] = useState("");
    const [ confirmPassword, setConfirmPassword ] = useState("");
    const [ newsletter, setNewsletter ] = useState(false);
    const [ privacyPolicy, setPrivacyPolicy ] = useState(false);
    const [ createUser ] = useMutation(
        REGISTER_USER,
        {
            variables: {
                email: email,
                firstName: firstName,
                lastName: lastName,
                password: password,
                isSubscribedNewsletter: newsletter
            },
            onCompleted: (data) => {
                if (data.applicationUser.createUser.result.succeeded) {
                    setErrorMessage(null);
                    setInfoMessage("You have been registered. You can now login with your new account.");
                } else {
                    let error = data.applicationUser.createUser.result.errors.map((e: any) => e.description).join(", ");
                    setInfoMessage(null);
                    setErrorMessage(error);
                }
            },
            onError: (data) => {
                setErrorMessage(data.message);
            }
        }
    );

    const valid = privacyPolicy && firstName !== "" && lastName !== "" && email !== "" && password !== "" && email === confirmEmail && password === confirmPassword;

    return <React.Fragment>
        <Section className={styles.intro}>
            <h1 className={styles.title}>Register User</h1>
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
                        <TextField onChange={(action) => setFirstName(action.target.value)} value={firstName} required label="First Name" type="text" />
                        <TextField onChange={(action) => setLastName(action.target.value)} value={lastName} required label="Last Name" type="text" />
                        <TextField onChange={(action) => setEmail(action.target.value)} value={email} required label="E-Mail Address" type="email" />
                        <TextField onChange={(action) => setConfirmEmail(action.target.value)} value={confirmEmail} error={email !== confirmEmail} required label="Confirm E-Mail Address" type="email" />
                        <TextField onChange={(action) => setPassword(action.target.value)} value={password} required label="Password" type="password" />
                        <TextField onChange={(action) => setConfirmPassword(action.target.value)} value={confirmPassword} error={password !== confirmPassword} required label="Confirm Password" type="password" />
                        <FormControlLabel
                            control={<Checkbox value={newsletter} onChange={(action) => setNewsletter(action.target.checked)} />}
                            label={<React.Fragment>I would like to receive an update from CollAction every once in a while - don't worry, we like spam as little as you do! <span role="img" aria-label="smiley">🙂</span></React.Fragment>} />
                        <FormControl error={!privacyPolicy}>
                            <FormControlLabel
                                control={<Checkbox value={privacyPolicy} onChange={(action) => setPrivacyPolicy(action.target.checked)} />}
                                label={<React.Fragment>I've read and agreed to our <Link to="/privacy-policy">privacy policy</Link></React.Fragment>}
                                />
                            <FormHelperText>You need to agree to our privacy policy</FormHelperText>
                        </FormControl>
                        <Button disabled={!valid} onClick={() => createUser()}>Register</Button>
                    </FormGroup>
                </Grid>
            </Grid>
        </Section>
    </React.Fragment>;
};

export default RegisterUserPage;

const REGISTER_USER = gql`
    mutation RegisterUser($email: String!, $firstName: String!, $lastName: String!, $password: String!, $isSubscribedNewsletter: Boolean!) {
        applicationUser {
            createUser(user: { email: $email, firstName: $firstName, lastName: $lastName, password: $password, isSubscribedNewsletter: $isSubscribedNewsletter}) {
                result {
                    succeeded
                    errors {
                        code
                        description
                    }
                }
            }
        }
    }
`;