import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import api from '../lib/axios'
import { FileText, Upload } from 'lucide-react'
import { useEffect } from 'react'
import { connection, startConnection } from '../lib/signalr'
const statusColor: Record<string, string> = {
  Draft: 'bg-gray-100 text-gray-600',
  Pending: 'bg-yellow-100 text-yellow-700',
  Approved: 'bg-green-100 text-green-700',
  Rejected: 'bg-red-100 text-red-700',
}

export default function DocumentsPage() {
  const queryClient = useQueryClient()
  useEffect(() => {
  startConnection()

  connection.on('DocumentStatusChanged', () => {
    queryClient.invalidateQueries({ queryKey: ['documents'] })
  })

  return () => {
    connection.off('DocumentStatusChanged')
  }
}, [])
  const [title, setTitle] = useState('')
  const [file, setFile] = useState<File | null>(null)
  const [uploading, setUploading] = useState(false)

  const { data: documents = [], isLoading } = useQuery({
    queryKey: ['documents'],
    queryFn: () => api.get('/Documents').then(r => r.data),
  })

  const uploadMutation = useMutation({
    mutationFn: async () => {
      if (!file || !title) return
      const form = new FormData()
      form.append('title', title)
      form.append('file', file)
      await api.post('/Documents', form)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['documents'] })
      setTitle('')
      setFile(null)
      setUploading(false)
    }
  })

  return (
    <div className="max-w-5xl mx-auto p-6">
      <div className="bg-white rounded-xl shadow p-6 mb-6">
        <h2 className="text-lg font-semibold mb-4 flex items-center gap-2">
          <Upload size={20} /> Upload tài liệu mới
        </h2>
        <input
          className="w-full border rounded-lg px-4 py-2 mb-3 focus:outline-none focus:ring-2 focus:ring-blue-500"
          placeholder="Tiêu đề tài liệu"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
        />
        <input
          type="file"
          accept=".pdf,.docx"
          className="mb-3"
          onChange={(e) => setFile(e.target.files?.[0] || null)}
        />
        <button
          className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition disabled:opacity-50"
          disabled={!title || !file}
          onClick={() => { setUploading(true); uploadMutation.mutate() }}
        >
          {uploading ? 'Đang upload...' : 'Upload'}
        </button>
      </div>

      <div className="bg-white rounded-xl shadow p-6">
        <h2 className="text-lg font-semibold mb-4 flex items-center gap-2">
          <FileText size={20} /> Danh sách tài liệu
        </h2>
        {isLoading ? (
          <p className="text-gray-500">Đang tải...</p>
        ) : documents.length === 0 ? (
          <p className="text-gray-400 text-center py-8">Chưa có tài liệu nào</p>
        ) : (
          <table className="w-full text-sm">
            <thead>
              <tr className="text-left border-b text-gray-500">
                <th className="pb-2">Tiêu đề</th>
                <th className="pb-2">Loại</th>
                <th className="pb-2">Trạng thái</th>
                <th className="pb-2">Ngày tạo</th>
              </tr>
            </thead>
            <tbody>
              {documents.map((doc: any) => (
                <tr key={doc.id} className="border-b hover:bg-gray-50">
                  <td className="py-3 font-medium">{doc.title}</td>
                  <td className="py-3 text-gray-500">{doc.fileType}</td>
                  <td className="py-3">
                    <span className={`px-2 py-1 rounded-full text-xs font-medium ${statusColor[doc.status]}`}>
                      {doc.status}
                    </span>
                  </td>
                  <td className="py-3 text-gray-500">
                    {new Date(doc.createdAt).toLocaleDateString('vi-VN')}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  )
}