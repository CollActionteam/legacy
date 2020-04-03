import { Grid, Container } from '@material-ui/core';
import { Field, Form, Formik } from 'formik';
import { TextField } from 'formik-material-ui';
import React from 'react';
import Helmet from 'react-helmet';
import { Button } from '../../../components/Button/Button';
import { RichTextEditorFormControl } from '../../../components/RichTextEditorFormContol';
import { Section } from '../../../components/Section';
import Categories from './Categories';
import styles from './Create.module.scss';
import { initialValues, validations } from './form';
import UploadImage from './UploadImage';

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
            <Section center color="grey" title="Start a new crowdaction" className={styles.title}>
              <p>Tell people about your crowdaction and why they should be excited!</p>
            </Section>
            <Section>
              <Grid container>
                <Grid item md={3}></Grid>
                <Grid item md={6} xs={12} >
                  <Container>
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
                  </Container>
                </Grid>
              </Grid>
            </Section>

            <Section color="grey">
              <Grid container spacing={3}>
                <Grid item md={7} xs={12}>
                  <Container className={styles.bannerContainer}>
                    <UploadImage></UploadImage>
                  </Container>
                </Grid>
                <Grid item md={5} xs={12}>
                  <Container>
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
                    <Field
                      name="hashtags"
                      label="Hashtags"
                      helperText="No #, seperate tags with ; e.g. tag1;tag2"
                      component={TextField}
                      className={styles.formRow}
                      fullWidth
                    >
                    </Field>
                  </Container>
                </Grid>
              </Grid>
            </Section>

            <Section>
              <Grid container>
                <Grid item md={7} xs={12}>
                  <Container>

                    <RichTextEditorFormControl
                      formik={props}
                      name="description"
                      label="Short description"
                      hint="E.g. reduce plastic waste and save our oceans!"
                      className={styles.formRow}
                      fullWidth
                    >
                    </RichTextEditorFormControl>
                    
                    <RichTextEditorFormControl
                      formik={props}
                      name="goal"
                      label="Goal/impact"
                      hint="What is the problem you are trying to solve?"
                      className={styles.formRow}
                      fullWidth
                    >
                    </RichTextEditorFormControl>

                    <p>Image here</p>
                                    
                    <RichTextEditorFormControl
                      formik={props}
                      name="comments"
                      label="Other comments (optional)"
                      hint="E.g. background, process, FAQs, about the initiator"
                      className={styles.formRow}
                      fullWidth
                    >
                    </RichTextEditorFormControl>
                    
                    <Field
                      name="youtube"
                      label="YouTube Video Link"
                      helperText="Descriptive video, e.g. http://www.youtube.com/watch?v=-wtIMTCHWuI"
                      component={TextField}
                      className={styles.formRow}
                      fullWidth
                    >                    
                    </Field>
                  </Container>
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