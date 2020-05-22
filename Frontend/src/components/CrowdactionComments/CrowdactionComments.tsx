import React, { useState, useEffect } from "react";
import { gql, useQuery, useMutation, DataProxy, FetchResult } from "@apollo/client";
import Loader from "../Loader/Loader";
import { Card, CardContent, Grid, CardActions } from "@material-ui/core";
import Formatter from "../../formatter";
import { Button } from "../Button/Button";
import { useUser } from '../../providers/UserProvider';
import { Alert } from "../Alert/Alert";
import { Form, Formik } from "formik";
import * as Yup from "yup";
import { RichTextEditorFormControl } from "../RichTextEditorFormContol/RichTextEditorFormControl";

interface ICrowdactionCommentsProps {
    id: string;
}

const DELETE_COMMENT = gql`
  mutation($commentId: ID!) {
    crowdaction {
      deleteComment(commentId: $commentId)
    }
  }
`;

const CREATE_COMMENT = gql`
  mutation($crowdactionId: ID!, $comment: String!) {
    crowdaction {
      createComment(crowdactionId: $crowdactionId, comment: $comment) {
        id
        comment
        commentedAt
        user {
          id
          fullName
        }
      }
    }
  }
`;

const GET_COMMENTS = gql`
  query($crowdactionId: ID!, $take: Int!, $skip: Int!) {
    crowdaction(id: $crowdactionId) {
      id
      comments(take: $take, skip: $skip, orderBy: { path: "commentedAt", descending: true }) @connection(key: "comment", filter: ["crowdactionId"]) {
        id
        comment
        commentedAt
        user {
          id
          fullName
        }
      }
    }
  }`;

export default ({ id }: ICrowdactionCommentsProps) => {
    const numCommentsPerFetch = 10;
    const user = useUser();
    const [ page, setPage ] = useState(0);
    const { data, loading, fetchMore, error } = useQuery<any>(
      GET_COMMENTS,
      {
        variables: {
          crowdactionId: id,
          take: numCommentsPerFetch,
          skip: 0
        }
      }
    );
    const [ deleteComment ] = useMutation(
      DELETE_COMMENT,
      {
        update: (cache: DataProxy, mutationResult: FetchResult) => {
          // Remove the comment from the cache
          if (mutationResult.data) {
            const removedId: number = mutationResult.data.crowdaction.deleteComment;
            for (var p = 0; p <= page; p++) {
              const vars = {
                crowdactionId: id,
                take: numCommentsPerFetch,
                skip: p * numCommentsPerFetch
              };
              const comments: any = cache.readQuery({
                query: GET_COMMENTS,
                variables: vars
              });
              const toRemove = 
                comments.crowdaction
                        .comments
                        .findIndex((c: any) => c.id === removedId);
              if (toRemove >= 0) {
                const currentComments = comments.crowdaction.comments;
                const filteredComments = currentComments.slice(0, toRemove)
                                                        .concat(currentComments.slice(toRemove + 1));
                cache.writeQuery({
                  query: GET_COMMENTS,
                  variables: vars,
                  data: {
                    crowdaction: {
                      __typename: comments.crowdaction.__typename,
                      id: id,
                      comments: filteredComments
                    }
                  }
                });
                return;
              }
            }
          }
        }
      });
    const [ createComment ] = useMutation(CREATE_COMMENT,
      {
        update: (cache: DataProxy, mutationResult: FetchResult) => {
          // Add the comment to the top of the comment list by putting it in the cache
          if (mutationResult.data) {
            const newComment = mutationResult.data.crowdaction.createComment;
            const vars = { crowdactionId: id, take: numCommentsPerFetch, skip: 0 };
            const currentComments: any = cache.readQuery({
              query: GET_COMMENTS,
              variables: vars
            });
            cache.writeQuery({
              query: GET_COMMENTS,
              variables: vars,
              data: {
                crowdaction: {
                  __typename: currentComments.crowdaction.__typename,
                  id: id,
                  comments: [ newComment, ...currentComments.crowdaction.comments ],
                }
              }
            });
          }
        }
      });
    const loadMore = () => {
      setPage(page + 1);
      fetchMore({
        variables: {
          skip: numCommentsPerFetch * (page + 1)
        },
        updateQuery: (previousResult, { fetchMoreResult }) => {
          const newCrowdaction = fetchMoreResult.crowdaction;
          const previousCrowdaction = previousResult.crowdaction;

          return {
            // Put the new comments at the end of the list and update `pageInfo`
            // so we have the new `endCursor` and `hasNextPage` values
            crowdaction: {
              __typename: previousCrowdaction.__typename,
              id: previousCrowdaction.id,
              comments: [...previousCrowdaction.comments, ...newCrowdaction.comments],
            }
          };
        }
      })        
    };

    useEffect(() => {
      if (error) {
        console.error(error?.message);
      }
    }, [ error ]);

    const validationSchema = Yup.object({
      comment: Yup.string()
    });
    
    const onSubmit = async (values: any, { setSubmitting, resetForm }: any) => { 
      await createComment({
        variables: { comment: values.comment, crowdactionId: id }
      });
      resetForm({});
      setSubmitting(false);
    };

    return <>
        <Grid container spacing={4}>
          <Alert type="error" text={error?.message} />
          {
            user ?
                <Grid item xs={12}>
                  <Formik initialValues={{ comment: '' }} validationSchema={validationSchema} onSubmit={onSubmit}>
                    {(formik) =>
                      <Form>
                        <Card>
                          <CardContent>
                            <RichTextEditorFormControl
                              formik={formik}
                              name="comment"
                              label="Comment"
                              hint="Comment about this crowdaction"
                              fullWidth />
                          </CardContent>
                          <CardActions>
                            <Button type="submit">Comment</Button>
                          </CardActions>
                        </Card>
                      </Form>
                    }
                  </Formik>
                </Grid>
                : null
          }
          {
            data?.crowdaction?.comments?.map((comment: any) => {
                const commentDate = new Date(comment.commentedAt);
                return <Grid key={comment.id} item xs={12}>
                  <Card>
                    <CardContent>
                      <h4>{ comment.user.fullName }</h4>
                      <span dangerouslySetInnerHTML={{ __html: comment.comment}} />
                      <p><em>{ Formatter.date(commentDate) } { Formatter.time(commentDate) }</em></p>
                    </CardContent>
                    {
                     user?.isAdmin ?
                      <CardActions>
                        <Button onClick={() => deleteComment({ variables: { commentId: comment.id }})}>Delete Comment</Button>
                      </CardActions> :
                      null
                    }
                  </Card>
                </Grid>;
              })
          }
          { loading ? <Grid item xs={12}><Loader /></Grid> : null }
          { data?.crowdaction?.comments.length % numCommentsPerFetch === 0 ? 
              <Grid item xs={12}><Button onClick={() => loadMore()}>Load More</Button></Grid> : 
              null
          }
        </Grid>
    </>;
}