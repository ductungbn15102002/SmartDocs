import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import api from '../lib/axios'
import { CheckCircle, XCircle, Clock, Send } from 'lucide-react'

const statusColor: Record<string, string> = {
  Draft: 'bg-gray-100 text-gray-600',
  Pending: 'bg-yellow-100 text-yellow-700',
  Approved: 'bg-green-100 text-green-700',
  Rejected: 'bg-red-100 text-red-700',
}

export default function ApprovalPage() {
  const queryClient = useQueryClient()
  const [selected, setSelected] = useState<any>(null)
  const [comment, setComment] = useState('')

  const { data: documents = [] } = useQuery({
    queryKey: ['documents'],
    queryFn: () => api.get('/Documents').then(r => r.data),
  })

  const submitMutation = useMutation({
    mutationFn: (docId: string) =>
      api.post(`/Documents/${docId}/submit`, { reviewerId: null }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['documents'] })
  })

  const reviewMutation = useMutation({
    mutationFn: ({ docId, isApproved }: { docId: string, isApproved: boolean }) =>
      api.post(`/Documents/${docId}/review`, {
        isApproved,
        comment: comment || (isApproved ? 'Approved' : 'Rejected'),
        tag: isApproved ? 0 : 3
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['documents'] })
      setSelected(null)
      setComment('')
    }
  })

  return (
    <div className="max-w-5xl mx-auto p-6">
      <h2 className="text-lg font-semibold mb-4">Approval Workflow</h2>

      <div className="grid grid-cols-3 gap-4 mb-6">
        {['Draft', 'Pending', 'Approved'].map(status => (
          <div key={status} className="bg-white rounded-xl shadow p-4 text-center">
            <p className="text-2xl font-bold text-blue-600">
              {documents.filter((d: any) => d.status === status).length}
            </p>
            <p className="text-sm text-gray-500">{status}</p>
          </div>
        ))}
      </div>

      <div className="bg-white rounded-xl shadow p-6">
        <table className="w-full text-sm">
          <thead>
            <tr className="text-left border-b text-gray-500">
              <th className="pb-2">Tiêu đề</th>
              <th className="pb-2">Trạng thái</th>
              <th className="pb-2">Ngày tạo</th>
              <th className="pb-2">Hành động</th>
            </tr>
          </thead>
          <tbody>
            {documents.map((doc: any) => (
              <tr key={doc.id} className="border-b hover:bg-gray-50">
                <td className="py-3 font-medium">{doc.title}</td>
                <td className="py-3">
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${statusColor[doc.status]}`}>
                    {doc.status}
                  </span>
                </td>
                <td className="py-3 text-gray-500">
                  {new Date(doc.createdAt).toLocaleDateString('vi-VN')}
                </td>
                <td className="py-3 flex gap-2">
                  {doc.status === 'Draft' && (
                    <button
                      onClick={() => submitMutation.mutate(doc.id)}
                      className="flex items-center gap-1 text-xs bg-blue-50 text-blue-600 px-2 py-1 rounded hover:bg-blue-100"
                    >
                      <Send size={12} /> Submit
                    </button>
                  )}
                  {doc.status === 'Pending' && (
                    <button
                      onClick={() => setSelected(doc)}
                      className="flex items-center gap-1 text-xs bg-yellow-50 text-yellow-600 px-2 py-1 rounded hover:bg-yellow-100"
                    >
                      <Clock size={12} /> Review
                    </button>
                  )}
                  {doc.status === 'Approved' && <CheckCircle size={16} className="text-green-500" />}
                  {doc.status === 'Rejected' && <XCircle size={16} className="text-red-500" />}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Review Modal */}
      {selected && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl shadow-xl p-6 w-full max-w-md">
            <h3 className="font-semibold text-lg mb-2">{selected.title}</h3>
            <p className="text-sm text-gray-500 mb-4">Nhập comment và chọn hành động</p>
            <textarea
              className="w-full border rounded-lg px-4 py-2 mb-4 focus:outline-none focus:ring-2 focus:ring-blue-500"
              rows={3}
              placeholder="Comment (không bắt buộc)"
              value={comment}
              onChange={(e) => setComment(e.target.value)}
            />
            <div className="flex gap-3">
              <button
                className="flex-1 bg-green-600 text-white py-2 rounded-lg hover:bg-green-700 flex items-center justify-center gap-2"
                onClick={() => reviewMutation.mutate({ docId: selected.id, isApproved: true })}
              >
                <CheckCircle size={16} /> Approve
              </button>
              <button
                className="flex-1 bg-red-500 text-white py-2 rounded-lg hover:bg-red-600 flex items-center justify-center gap-2"
                onClick={() => reviewMutation.mutate({ docId: selected.id, isApproved: false })}
              >
                <XCircle size={16} /> Reject
              </button>
              <button
                className="px-4 py-2 border rounded-lg hover:bg-gray-50"
                onClick={() => setSelected(null)}
              >
                Huỷ
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}