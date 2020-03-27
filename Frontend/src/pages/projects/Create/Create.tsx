import { Grid } from '@material-ui/core';
import { Field, Form, Formik } from 'formik';
import { TextField } from 'formik-material-ui';
import React from 'react';
import Helmet from 'react-helmet';
import { Button } from '../../../components/Button/Button';
import { Section } from '../../../components/Section';
import Categories from './Categories';
import styles from './Create.module.scss';
import { initialValues, validations } from './form';


const CreateProjectPage = () => {
  
  const handleSubmit = (values: any) => {
    console.log(values);
  }
 
  return (
    <React.Fragment>
      <Helmet>
        <title>Create a project</title>
        <meta name="description" content="Create a project" />
      </Helmet>

      <Formik
        initialValues={initialValues}
        validationSchema={validations}
        validateOnChange={false}
        validateOnMount={false}
        validateOnBlur={true}        
        onSubmit={handleSubmit}
      >
        {(props) => (
          <Form>
            <Section>
              <Grid container>
                <Grid item md={3}></Grid>
                <Grid item md={6} xs={12} >
                  <Field 
                      name="projectName" 
                      label="Project name"
                      component={TextField}
                      className={styles.formRow}
                      fullWidth
                  >
                  </Field>                  
                  <Field
                    name="proposal"
                    label="Proposal"
                    multiline
                    rows="4"
                    helperText={`e.g. "If X people commit to Y, we'll all do it together!"`} 
                    component={TextField}
                    className={styles.formRow}
                    fullWidth
                  />
                  <Categories props={props}></Categories>
                </Grid>
              </Grid>
            </Section>

            <Section color="grey">
              <Grid container>
                <Grid item md={7} xs={12}>
                  <p>Image here</p>
                </Grid>
                <Grid item md={5} xs={12}>
                  <Field
                    name="target"
                    label="Target"
                    type="number"
                    helperText="The minimum number of people that must participate in your crowdaction" 
                    component={TextField}
                    className={styles.formRow}
                    fullWidth
                  >
                  </Field>
                  <Field
                    name="startDate"
                    label="Sign up opens"
                    type="date"
                    component={TextField}
                    InputLabelProps={{
                      shrink: true
                    }}
                    className={styles.formRow}
                    fullWidth
                  >
                  </Field>
                  <Field
                    name="endDate"
                    label="Sign up closes"
                    type="date"
                    helperText="This crowdaction will only start if it reaches its goal by this date, 00:00 GMT"
                    component={TextField}
                    InputLabelProps={{
                      shrink: true
                    }}
                    className={styles.formRow}
                    fullWidth
                  >
                  </Field>
            </Grid>
          </Grid>

            </Section>

            <Section>
              <Grid container>
                <Grid item xs={12}>
                  <div className={styles.submit}>
                    <Button type="submit">Submit project</Button>  
                  </div>
                </Grid>
              </Grid>
            </Section>

          </Form>
        )}
      </Formik>
    </React.Fragment>
  )
};

export default CreateProjectPage;