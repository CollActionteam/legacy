import React from 'react';
import Helmet from 'react-helmet';
import { Section } from '../../../components/Section';
import { Grid, TextField } from '@material-ui/core';
import { Button } from '../../../components/Button/Button';
import { Formik, Form, Field } from 'formik';

import styles from './Create.module.scss';

const CreateProjectPage = () => {

  const initialValues = {
    projectName: 'Hello'
  };
  
  const handleSubmit = (values: any) => {
    // Now; why isn't the form binding??
    console.log(values);
  }
    
  return (
    <React.Fragment>
      <Helmet>
        <title>Create a project</title>
        <meta name="description" content="Create a project" />
      </Helmet>

      <Formik
        initialValues={{
          projectName: 'Hello peeps'
        }}
        onSubmit={handleSubmit}
      >
        {props => (
          <Form>
            <Section>
              <Field 
                  name="projectName" 
                  component={TextField}
                  label="Project name"
                  fullWidth
              >
              </Field>
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



      {/* <form>
        <Section>
          <Grid container>
            <Grid item md={3}></Grid>
            <Grid item md={6} xs={12}>
              <div className={styles.formRow}>
                <Field
                  name="projectName"
                  label="Project name"
                  component={TextField}
                  fullWidth
                />
              </div>
              <div className={styles.formRow}>
                <TextField 
                  name="proposal" 
                  label="Proposal" 
                  multiline 
                  rows="4" 
                  fullWidth 
                  helperText={`e.g. "If X people commit to Y, we'll all do it together!"`} 
                  inputRef={register({ required: true })}
                ></TextField>
              </div>
              <div className={styles.formRow}>
                <FormControl fullWidth>
                  <InputLabel htmlFor="category">Category</InputLabel>
                  <Select 
                    name="category"
                    inputRef={register({ required: true })}
                  >
                    {categories
                          ? categories.__type.enumValues.map((c: any) => (
                              <MenuItem key={c.name} value={c.name}>
                                {Utils.formatCategory(c.name)}
                              </MenuItem>
                            ))
                          : null}
                  </Select>
                </FormControl>
              </div>
            </Grid>
          </Grid>
        </Section>

        <Section color="grey">
          <Grid container>
            <Grid item md={7} xs={12}>
              <p>Image here</p>
            </Grid>
            <Grid item md={5} xs={12}>
              <div className={styles.formRow}>
                <TextField name="target" label="Target" type="number" helperText="The minimum number of people that must participate in your crowdaction" fullWidth></TextField>
              </div>
              <div className={styles.formRow}>
                <TextField name="startDate" label="Sign up opens" type="date" fullWidth InputLabelProps={{shrink: true }}></TextField>
              </div>
              <div className={styles.formRow}>
                <TextField name="endDate" label="Sign up closes" type="date" fullWidth InputLabelProps={{shrink: true }}></TextField>
              </div>
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
      </form> */}

export default CreateProjectPage;