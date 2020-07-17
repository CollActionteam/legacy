import { Container, FormHelperText, Grid, InputLabel } from '@material-ui/core';
import { Field, Form, Formik, FormikProps } from 'formik';
import { TextField } from 'formik-material-ui';
import React from 'react';
import { Helmet } from 'react-helmet';
import { Button } from '../../../components/Button/Button';
import { CrowdactionStarter } from '../../../components/CrowdactionStarter/CrowdactionStarter';
import { RichTextEditorFormControl } from '../../../components/RichTextEditorFormContol/RichTextEditorFormControl';
import { Section } from '../../../components/Section/Section';
import { useUser } from '../../../providers/UserProvider';
import Categories from './Categories';
import styles from './Create.module.scss';
import { initialValues, ICrowdactionForm, validations } from './form';
import UploadImage from './UploadImage';
import { Link, useHistory } from 'react-router-dom';
import { gql, useMutation } from '@apollo/client';
import Loader from '../../../components/Loader/Loader';
import Utils from '../../../utils';

const CreateCrowdactionPage = () => {
  const user = useUser();
  const history = useHistory();

  const validate = async (props: FormikProps<any>) => {
    const errors = Object.keys(await props.validateForm());

    if (errors.length) {
      const el = document.getElementsByName(errors[0])[0];
      if (el) {
        el.scrollIntoView();
      }
    }
  };

  const [createCrowdaction] = useMutation(gql`
    mutation Create($crowdaction: NewCrowdactionInputGraph!) {
      crowdaction {
        createCrowdaction(crowdaction: $crowdaction) {
          succeeded
          crowdaction {
            nameNormalized
            id
          }
          errors {
            errorMessage
            memberNames
          }
        }
      }
    }
  `);

  const handleSubmit = async (form: ICrowdactionForm, { setSubmitting, setStatus }: any) => {
    setSubmitting(true);
    setStatus(null);

    var bannerId: number | null = null;
    var imageId: number | null = null;
    var cardId: number | null = null;
    try {
      bannerId = form.banner ? await Utils.uploadImage(form.banner, form.crowdactionName, 1600) : null;
      imageId = form.image ? await Utils.uploadImage(form.image, form.imageDescription, 1600) : null;
      cardId = form.banner ? await Utils.uploadImage(form.banner, form.crowdactionName, 370) : null;
    }
    catch (error) {
      console.error(error.toString());
      setStatus([ error ]);
      return;
    }

    const response = await createCrowdaction({
      variables: {
        crowdaction: {
          name: form.crowdactionName,
          bannerImageFileId: bannerId,
          cardImageFileId: cardId,
          categories: [form.category, form.secondCategory].filter(c => c !== ''),
          target: form.target,
          proposal: form.proposal,
          description: form.description,
          start: form.startDate,
          end: form.endDate,
          goal: form.goal,
          tags: form.tags ? form.tags.split(';') : [],
          creatorComments: form.comments || null,
          descriptiveImageFileId: imageId || null,
          descriptionVideoLink: form.youtube || null
        }
      }
    });

    if (!response || !response.data) {
      throw new Error('No response from graphql endpoint');
    }

    const result = response.data.crowdaction.createCrowdaction;

    if (!result.succeeded) {
      setStatus(result.errors);
      return;
    }

    history.push(`/crowdactions/create/thankyou/${result.crowdaction.id}`);
  }

  const pleaseLogin = (
    <>
      <Helmet>
        <title>Create a crowdaction</title>
        <meta name="description" content="Create a crowdaction" />
      </Helmet>
      <Section title="Start a new crowdaction" className={styles.title}>
        <Container className={styles.login}>
          <p>Please <Link to="/account/login?returnUrl=/crowdactions/create">login</Link> first!</p>
        </Container>
      </Section>
    </>
  )

  const createCrowdactionForm = (
    <>
      <Helmet>
        <title>Create a crowdaction</title>
        <meta name="description" content="Create a crowdaction" />
      </Helmet>

      <Formik
        initialValues={initialValues}
        validationSchema={validations}
        validateOnChange={false}
        validateOnMount={false}
        validateOnBlur={true}
        onSubmit={async (values, actions) => handleSubmit(values, actions)}
      >
        {(props: FormikProps<ICrowdactionForm>) => (
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
                        name="crowdactionName"
                        label="Title"
                        helperText="Write a clear, brief title that helps people quickly understand the idea behind the crowdaction"
                        component={TextField}
                        className={styles.formRow}
                        fullWidth
                    >
                    </Field>
                    <Field
                      name="proposal"
                      label="Objective"
                      helperText="Describe in a few words what the crowdaction aims to achieve"
                      multiline
                      rows="4"
                      component={TextField}
                      className={styles.formRow}
                      fullWidth
                    />
                    <Categories props={props} />
                  </Container>
                </Grid>
              </Grid>
            </Section>

            <Section color="grey">
              <Grid container spacing={3}>
                <Grid item md={7} xs={12}>
                  <Container className={styles.bannerContainer}>
                    <UploadImage name="banner" formik={props}></UploadImage>
                  </Container>
                </Grid>
                <Grid item md={5} xs={12}>
                  <Container>
                    <Field
                      name="target"
                      label="Goal amount"
                      helperText="Set an achievable number of people to join your crowdaction"
                      type="number"
                      component={TextField}
                      className={styles.formRow}
                      fullWidth
                    >
                    </Field>
                    <Field
                      name="startDate"
                      label="Launch date"
                      type="date"
                      helperText="Set a date from which people will be able to join the crowdaction"
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
                      label="Sign-up duration"
                      type="date"
                      helperText="Set a specific end date for when the sign-up closes. Sign-up closes on 00:00 GMT on this date"
                      component={TextField}
                      InputLabelProps={{
                        shrink: true
                      }}
                      className={styles.formRow}
                      fullWidth
                    >
                    </Field>
                    <Field
                      name="tags"
                      label="Hashtags"
                      helperText="No #, separate tags with ; e.g. tag1;tag2"
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
                      label="Description"
                      hint="Describe what you're gathering participants for. Your description should be convincing and tell people everything they need to know. Get creative but be specific and be clear!"
                      className={styles.formRow}
                      fullWidth
                    >
                    </RichTextEditorFormControl>

                    <RichTextEditorFormControl
                      formik={props}
                      name="goal"
                      label="Goal"
                      hint="Describe in more detail what you want to achieve with your crowdaction. What is the problem you are trying to solve?"
                      className={styles.formRow}
                      fullWidth
                    >
                    </RichTextEditorFormControl>

                    <div className={styles.formRow}>
                      <InputLabel className={styles.label} shrink>
                        Infograph or image
                      </InputLabel>
                      <UploadImage name="image" formik={props}></UploadImage>
                      {!props.values.image &&
                        <FormHelperText>Add an infograph or another descriptive image to support your description and your goal.</FormHelperText>
                      }
                    </div>


                    {props.values.image &&
                      <Field
                        name="imageDescription"
                        label="Image description"
                        helperText="Provide a short description to go with the image"
                        component={TextField}
                        className={styles.formRow}
                        fullWidth
                      >
                      </Field>
                  }

                    <RichTextEditorFormControl
                      formik={props}
                      name="comments"
                      label="Other comments (optional)"
                      hint="Anything else you want to tell about your crowdaction, e.g. the background, more about the process, why you care about this action, FAQs etc."
                      className={styles.formRow}
                      fullWidth
                    >
                    </RichTextEditorFormControl>

                    <Field
                      name="youtube"
                      label="YouTube video link"
                      helperText="A video to go with your crowdaction. Use the format https://www.youtube.com/embed/-wtIMTCHWuI"
                      component={TextField}
                      className={styles.formRow}
                      fullWidth
                    >
                    </Field>
                  </Container>
                </Grid>

                <Grid item md={5} xs={12}>
                  <Container>
                    <CrowdactionStarter user={user}></CrowdactionStarter>
                  </Container>
                </Grid>
              </Grid>
            </Section>

            <Section>
              <Grid container>
                <Grid item xs={12}>
                  <div className={styles.submit}>
                    { props.isSubmitting
                      ? <Loader></Loader>
                      : <Button
                          type="submit"
                          disabled={props.isSubmitting}
                          onClick={() => validate(props)}
                        >
                          Submit crowdaction
                        </Button>
                    }
                    { props.status &&
                      <div className={styles.submitErrors}>
                        <ul>
                          { props.status.filter((e: any) => e !== undefined).map((error: any, idx: number) => (
                            <li key={idx}>{ error.errorMessage }</li>
                          ))}
                        </ul>
                      </div>
                    }
                  </div>
                </Grid>
              </Grid>
            </Section>
          </Form>
          )}
      </Formik>
    </>
  );

  return user ? createCrowdactionForm : pleaseLogin;
};

export default CreateCrowdactionPage;