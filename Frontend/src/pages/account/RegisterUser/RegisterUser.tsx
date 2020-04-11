import React, { useState } from "react";
import { Section } from "../../../components/Section/Section";
import { Alert } from "../../../components/Alert/Alert";
import { Grid, Checkbox, FormGroup, TextField, FormControlLabel, FormControl } from "@material-ui/core";
import { gql, useMutation } from "@apollo/client";
import styles from "./RegisterUser.module.scss";
import { Link } from "react-router-dom";
import { useFormik, FormikProvider, Form } from "formik";
import * as Yup from "yup";
import { Button } from "../../../components/Button/Button";
import Helmet from "react-helmet";

const REGISTER_USER = gql`
    mutation RegisterUser($user: NewUserInputGraph!) {
        user {
            createUser(user: $user) {
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

const RegisterUserPage = () => {
    const [ errorMessage, setErrorMessage ] = useState<string | null>(null);
    const [ infoMessage, setInfoMessage ] = useState<string | null>(null);
    const [ createUser ] = useMutation(
        REGISTER_USER,
        {
            onCompleted: (data) => {
                if (data.user.createUser.result.succeeded) {
                    setErrorMessage(null);
                    setInfoMessage("You have been registered. You can now login with your new account.");
                } else {
                    let error = data.user.createUser.result.errors.map((e: any) => e.description).join(", ");
                    setInfoMessage(null);
                    setErrorMessage(error);
                    console.error(error);
                }
            },
            onError: (data) => {
                setErrorMessage(data.message);
                console.error(data.message);
            }
        }
    );
    const formik = useFormik({
        initialValues: {
            firstName: "",
            lastName: "",
            email: "",
            confirmEmail: "",
            password: "",
            confirmPassword: "",
            isSubscribedNewsletter: false,
            privacyPolicy: false
        },
        validationSchema: Yup.object({
            firstName: Yup.string(),
            lastName: Yup.string(),
            email: Yup.string().required("You must fill in your e-mail address")
                               .email("You must fill in a valid email address"),
            confirmEmail: Yup.string().required("You must fill in the confirmation e-mail address")
                                      .when("email", {
                is: val => val?.length > 0,
                then: Yup.string()
                         .oneOf([Yup.ref("email")], "Both e-mails must be the same")
            }),
            password: Yup.string().min(6, "Password must be at least six characters long")
                                  .required("Must fill in a password at least six characters long"),
            confirmPassword: Yup.string().required("Must fill in a confirmation password")
                                         .when("password", {
                is: val => val?.length > 0,
                then: Yup.string()
                         .oneOf([Yup.ref("password")], "Both passwords need to be the same")
                         .required("Must fill in a confirmation password")
            }),
            isSubscribedNewsletter: Yup.bool(),
            privacyPolicy: Yup.bool().oneOf([true], "You must agree to our privacy policy")
        }),
        onSubmit: (values) => {
            createUser({
                variables: {
                    user: {
                        firstName: values.firstName,
                        lastName: values.lastName,
                        email: values.email,
                        password: values.password,
                        isSubscribedNewsletter: values.isSubscribedNewsletter
                    }
                }
            });
        }
    });

    return <React.Fragment>
        <Helmet>
          <title>Register User</title>
          <meta name="description" content="Register User" />
        </Helmet>
        <Section className={styles.intro}>
            <h1 className={styles.title}>Register User</h1>
        </Section>
        <Alert type="error" text={errorMessage} />
        <Alert type="info" text={infoMessage} />
        <Section color="grey">
            <Grid container justify="center">
                <Grid item sm={6}>
                    <FormikProvider value={formik}>
                        <Form onSubmit={formik.handleSubmit}>
                            <FormGroup>
                                <TextField name="firstName" label="First Name" type="text" { ... formik.getFieldProps('firstName') } />
                                <TextField name="lastName" label="Last Name" type="text" { ...formik.getFieldProps('lastName') } />
                                <TextField name="email" label="E-Mail" type="text" error={formik.touched.email && formik.errors.email !== undefined} { ...formik.getFieldProps('email') } />
                                { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.email} /> : null }
                                <TextField name="confirmEmail" label="Confirm E-Mail" type="text" error={formik.touched.confirmEmail && formik.errors.confirmEmail !== undefined} { ...formik.getFieldProps('confirmEmail') } />
                                { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.confirmEmail} /> : null }
                                <TextField name="password" label="Password" type="password" error={formik.touched.password && formik.errors.password !== undefined} { ...formik.getFieldProps('password') } />
                                { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.password} /> : null }
                                <TextField name="confirmPassword" label="Confirm Password" type="password" error={formik.touched.confirmPassword && formik.errors.confirmPassword !== undefined} { ...formik.getFieldProps('confirmPassword') } />
                                { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.confirmPassword} /> : null }
                                <FormControlLabel
                                    control={<Checkbox name="isSubscribedNewsletter" { ...formik.getFieldProps('isSubscribedNewsletter')} />}
                                    label={<React.Fragment>I would like to receive an update from CollAction every once in a while - don't worry, we like spam as little as you do! <span role="img" aria-label="smiley">🙂</span></React.Fragment>} />
                                <FormControl>
                                    <FormControlLabel
                                        control={<Checkbox name="privacyPolicy" { ...formik.getFieldProps('privacyPolicy') } />}
                                        label={<React.Fragment>I've read and agreed to our <Link to="/privacy-policy">privacy policy</Link></React.Fragment>}
                                        />
                                    { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.privacyPolicy} /> : null }
                                </FormControl>
                                <Button type="submit">Register</Button>
                            </FormGroup>
                        </Form>
                    </FormikProvider>
                </Grid>
            </Grid>
        </Section>
    </React.Fragment>;
};

export default RegisterUserPage;